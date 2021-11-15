using Dapper;
using MySqlConnector;
using SecureChatServer.Models;
using System.Data;
using Xunit;

namespace SecureChatServer.Helpers
{
    public interface IChatService
    {
        Task<List<PreviouslyContactedUser>> GetPreviouslyContacted(int uid);
        Task<bool> CheckValidChatRelationId(int uid, int chat_relation_id);
        Task<List<Message>> GetConversation(int chat_relation_id);
        Task InitializeChat(int src_uid, int dest_uid);
        Task SendMessage(Message message);
    }
    public class ChatService : IChatService
    {
        IConfiguration _configuration;
        public ChatService(IConfiguration config)
        {
            _configuration = config;
        }

        public async Task<bool> CheckValidChatRelationId(int uid, int chat_relation_id)
        {
            string sql = "select COUNT(*) from chat_relations where (chat_by=@current_user and id=@chat_relation_id) or " +
                                                            "chat_with=@current_user and id=@chat_relation_id";
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                try
                {
                    int relations = await connection.ExecuteScalarAsync<int>(sql, new { current_user = uid, chat_relation_id = chat_relation_id });

                    return relations == 1;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public async Task<List<Message>> GetConversation(int chat_relation_id)
        {
            string sql = "select * from messages where chat_relation_id=@chat_relation_id;";
            using(var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                IEnumerable<Message> messages = await connection.QueryAsync<Message>(sql, new {chat_relation_id = chat_relation_id});
                if (messages == null)
                    return new List<Message>();
                return new List<Message>(messages);
            }
        }

        public async Task<List<PreviouslyContactedUser>> GetPreviouslyContacted(int uid)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                IEnumerable<PreviouslyContactedUser> users = await connection.QueryAsync<PreviouslyContactedUser>("GetContacted", new { arg1 = uid }, commandType: CommandType.StoredProcedure);
                if (users == null)
                {
                    return new List<PreviouslyContactedUser>();
                }
                return new List<PreviouslyContactedUser>(users);
            }
        }

        public async Task InitializeChat(int src_uid, int dest_uid)
        {
            string sql = "insert into chat_relations(chat_by, chat_with) values (@src_uid, @dest_uid)";
            using(var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                int result = await connection.ExecuteAsync(sql, new { src_uid = src_uid, dest_uid = dest_uid });
                Assert.True(result == 1);
            }
        }

        public async Task SendMessage(Message message)
        {
            string sql = "insert into messages(sender_id, receiver_id, chat_relation_id, message, message_type_id, message_at) values" +
                                            " (@sender_id, @receiver_id, @chat_relation_id, @message, @message_type_id, @message_at)";
            using(var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                int result = await connection.ExecuteAsync(sql, message);
                Assert.True(result == 1);
            }
        }
    }
}
