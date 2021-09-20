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
    [Route("api/[controller]")]
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
        public async Task<IActionResult> AuthenticateByEmailAsync(AuthenticateEmailRequest request)
        {
            AuthenticateResponse resp = await _userservice.AuthenticateByEmailAsync(request);
            if(resp == null)
            {
                return BadRequest(new { message = "Email or PrivateKey is incorrect" });
            }

            return Ok(resp);
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserFromEmail/{email}")]
        public async Task<User> GetUserFromEmail(string email)
        {
            string sql = "select * from users where email=@argemail;";
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                IEnumerable<User> user = await connection.QueryAsync<User>(sql, new { argemail = email });
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
