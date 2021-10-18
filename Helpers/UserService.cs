using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using SecureChatServer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace SecureChatServer.Helpers
{
    public interface IUserService
    {
        Task<AuthenticateResponse?> AuthenticateByEmailAsync(AuthenticateEmailRequest request);
        Task<RegisterResponse?> RegisterByEmailAsync(RegisterEmailRequest request);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
    }
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;
        public UserService(IConfiguration configuration, IOptions<AppSettings> options)
        {
            _configuration = configuration;
            _appSettings = options.Value;
        }
        public async Task<AuthenticateResponse?> AuthenticateByEmailAsync(AuthenticateEmailRequest request)
        {
            try
            {
                User user = await GetByEmailAsync(request.Email);
                if (user == null || user.uid == -1)
                {
                    return null;
                }

                byte[] data = Encoding.UTF8.GetBytes("SecureChatServer");
                byte[] signature = Convert.FromBase64String(request.signedText);
                int bytesRead = 0;
                RSA rsa = RSA.Create();
                rsa.ImportFromPem(user.public_key);
                //rsa.ImportRSAPublicKey(Encoding.UTF8.GetBytes(user.public_key), out bytesRead);
                Console.WriteLine($"{bytesRead} bytes read from public key");
                bool res = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                if (!res)
                {
                    return null;
                }

                string token = generateJwtToken(user);
                return new AuthenticateResponse
                {
                    Token = token,
                    email = user.email,
                    phone_number = user.phone_number,
                    username = user.username
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<RegisterResponse?> RegisterByEmailAsync(RegisterEmailRequest request)
        {
            if(request.email == null || request.public_key == null)
            {
                return new RegisterResponse { status = "Email or Public Key not provided" };
            }
            if (request.username == null)
                request.username = "";
            if (request.phone_number == null)
                request.phone_number = "";

            string sql = "insert into users(email, phone_number, username, public_key) " +
                "values (@email, @phone_number, @username, @public_key)";
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                var result = await connection.ExecuteAsync(sql, request);
                Assert.True(result == 1, "affected rows should be one");
            }
            return new RegisterResponse { status = "Registered Successfully" };
        }

        private string generateJwtToken(User user)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_appSettings.JwtSymmetricKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[] {
                    new Claim("uid", user.uid.ToString()),
                    new Claim("email", user.email),
                    new Claim("phone_number", user.phone_number),
                    new Claim("username", user.username),
                    new Claim("public_key", user.public_key)
                }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                    )
            };

            SecurityToken token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            string sql = "select * from users where email=@argemail;";
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                IEnumerable<User> user = await connection.QueryAsync<User>(sql, new { argemail = email });
                if (user == null || user.Count() != 1)
                {
                    return new User();
                }
                User ans = user.Single();
                return ans;
            }
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            string sql = "select * from users where username=@arg_username;";
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                IEnumerable<User> user = await connection.QueryAsync<User>(sql, new { arg_username = username });
                if (user == null || user.Count() != 1)
                {
                    return new User();
                }
                User ans = user.Single();
                return ans;
            }
        }
    }
}
