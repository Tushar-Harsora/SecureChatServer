using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureChatServer.Models;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using MySqlConnector;
using System.Text.Json;
using System.Text.Json.Serialization;
using SecureChatServer.Helpers;

namespace SecureChatServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userservice;
        public UserController(IConfiguration configuration, IUserService userservice)
        {
            _configuration = configuration;
            _userservice = userservice;
        }

        [HttpPost]
        [Route("AuthenticateByEmail")]
        public async Task<IActionResult> AuthenticateByEmailAsync(AuthenticateEmailRequest request)
        {
            AuthenticateResponse? resp = await _userservice.AuthenticateByEmailAsync(request);
            if(resp == null)
            {
                return BadRequest(new { message = "Email or PrivateKey is incorrect" });
            }

            return Ok(resp);
        }

        [HttpPost]
        [Route("RegisterByEmail")]
        public async Task<IActionResult> RegisterByEmailAsync(RegisterEmailRequest request)
        {
            RegisterResponse? res = await _userservice.RegisterByEmailAsync(request);

            if(res == null)
            {
                return BadRequest(new RegisterResponse { status = "Email or Public Key not provided" });
            }

            return Ok(res);
        }

        [HttpPost]
        [Route("GetUserFromEmail")]
        public async Task<User> GetUserFromEmail(GetUserFromEmailRequest email)
        {
            string sql = "select * from users where email=@argemail;";
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                IEnumerable<User> user = await connection.QueryAsync<User>(sql, new { argemail = email.email });
                if(user == null || user.Count() != 1)
                {
                    return new User();
                }
                User ans = user.Single();
                return ans;
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserFromUsername/{username}")]
        public async Task<User> GetUserFromUsername(string username)
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
