namespace SecureChatServer.Models
{
    public class Message
    {
        public int id { get; set; }
        public int sender_id { get; set; }
        public int reciever_id { get; set; }
        public int chat_relation_id { get; set; }
        public string message { get; set; }
        public int message_type_id { get; set; }
        public DateTime message_at { get; set; }
    }
}
