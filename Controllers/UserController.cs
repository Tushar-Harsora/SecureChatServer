using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureChatServer.Models;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using MySqlConnector;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecureChatServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            //_logger = logger;
        }

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
