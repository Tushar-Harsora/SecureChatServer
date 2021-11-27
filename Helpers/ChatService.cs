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
        Task<List<Message>> GetConversation(int chat_relation_id, int current_user_id);
        Task InitializeChat(int src_uid, int dest_uid);
        Task<int> SendMessage(Message message);
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

        public async Task<List<Message>> GetConversation(int chat_relation_id, int current_user_id)
        {
            using(var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                try
                {
                    IEnumerable<Message> messages = await connection.QueryAsync<Message>("GetConversation", new { chat_relation = chat_relation_id, current_user_id = current_user_id },
                                                                                            commandType: CommandType.StoredProcedure);
                    if (messages == null)
                        return new List<Message>();
                    return new List<Message>(messages);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }

        public async Task<List<PreviouslyContactedUser>> GetPreviouslyContacted(int uid)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                try
                {
                    IEnumerable<PreviouslyContactedUser> users = await connection.QueryAsync<PreviouslyContactedUser>("GetContacted", new { arg1 = uid }, commandType: CommandType.StoredProcedure);
                    if (users == null)
                    {
                        return new List<PreviouslyContactedUser>();
                    }
                    return new List<PreviouslyContactedUser>(users);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
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

        public async Task<int> SendMessage(Message message)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("Default")))
            {
                var param = new DynamicParameters();
                param.Add("@sender_id", message.sender_id, DbType.Int32, ParameterDirection.Input);
                param.Add("@receiver_id", message.receiver_id, DbType.Int32, ParameterDirection.Input);
                param.Add("@chat_relation_id", message.chat_relation_id, DbType.Int32, ParameterDirection.Input);
                param.Add("@message", message.message, DbType.String, ParameterDirection.Input);
                param.Add("@message_type_id", message.message_type_id, DbType.Int32, ParameterDirection.Input);
                param.Add("@message_at", message.message_at, DbType.DateTime, ParameterDirection.Input);
                param.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                int result = await connection.ExecuteAsync("SendMessage", param, commandType: CommandType.StoredProcedure);
                int return_val = param.Get<Int32>("@retval");
                return return_val;
            }
        }
    }
}
