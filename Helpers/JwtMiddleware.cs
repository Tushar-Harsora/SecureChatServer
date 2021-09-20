using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureChatServer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SecureChatServer.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appsettings)
        {
            _next = next;
            _appSettings = appsettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userservice)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if(token != null)
            {
                InsertUserInContext(context, userservice, token);
            }
            await _next(context);
        }

        private void InsertUserInContext(HttpContext context, IUserService userservice, string token)
        {
            try
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.UTF8.GetBytes(_appSettings.JwtSymmetricKey);

                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validToken);

                JwtSecurityToken jwt = (JwtSecurityToken)validToken;
                User this_user = new User()
                {
                    uid = int.Parse(jwt.Claims.First(claim => claim.Type == "uid").Value),
                    username = jwt.Claims.First(claim => claim.Type == "username").Value,
                    email= jwt.Claims.First(claim => claim.Type == "email").Value,
                    phone_number= jwt.Claims.First(claim => claim.Type == "phone_number").Value,
                    public_key = jwt.Claims.First(claim => claim.Type == "public_key").Value
                };

                context.Items["User"] = this_user;
            }
            catch
            {
                // Do Nothing as try block throws only when token or key is invalid
                // and User object will not be added to context and it would result in 
                // Unauthorized response on Protected Controller and Methods
            }
        }
    }
}
