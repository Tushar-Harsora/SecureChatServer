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
        [Route("/api/[controller]/{email}")]
        public async Task<User> GetUserFromEmail(string email)
        {
            string sql = "select * from users where email=@argemail;";
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                IEnumerable<User> user = await connection.QueryAsync<User>(sql, new { argemail = email });
                if(user == null)
                {
                    return new User();
                }
                User ans = user.Single();
                Console.WriteLine(ans.username + " " + ans.email);
                string ret = JsonSerializer.Serialize(ans);
                return ans;
            }
        }
    }
}
