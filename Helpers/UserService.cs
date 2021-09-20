using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using SecureChatServer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecureChatServer.Helpers
{
    public interface IUserService
    {
        Task<AuthenticateResponse> AuthenticateByEmailAsync(AuthenticateEmailRequest request);
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
        public async Task<AuthenticateResponse> AuthenticateByEmailAsync(AuthenticateEmailRequest request)
        {
            // TODO: Implement authentication by Checking correct message sign
            User user = await GetByEmailAsync(request.Email);
            if(user == null || user.uid == -1)
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
