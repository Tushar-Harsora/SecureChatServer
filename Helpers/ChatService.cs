using Dapper;
using MySqlConnector;
using SecureChatServer.Models;

namespace SecureChatServer.Helpers
{
    public interface IChatService
    {
        Task<List<User>> GetPreviouslyContacted(int uid);
    }
    public class ChatService : IChatService
    {
        IConfiguration _configuration;
        public ChatService(IConfiguration config)
        {
            _configuration = config;
        }
        public async Task<List<User>> GetPreviouslyContacted(int uid)
        {
            string sql = "select * from chat_relations where sender_id=@uid or receiver_id=@uid;";
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                IEnumerable<User> user = await connection.QueryAsync<User>(sql, new { uid = uid });
                if (user == null)
                {
                    return new List<User>();
                }
                return new List<User>(user);
            }
        }
    }
}
