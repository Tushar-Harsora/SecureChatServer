namespace SecureChatServer.Models
{
    public class PreviouslyContactedUser
    {
        public int uid { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string username { get; set; }
        public int chat_relation_id { get; set; }

        public int unread_counts { get; set; }

        public PreviouslyContactedUser()
        {
            uid = -1;
            email = "example@example.com";
            phone_number = "9999999999";
            username = "example";
            chat_relation_id = -1;
            unread_counts = 0;
        }
        public PreviouslyContactedUser(int uid, string email, string phone_number, string username, int chat_relation_id, int unread_counts)
        {
            this.uid = uid;
            this.email = email;
            this.phone_number = phone_number;
            this.username = username;
            this.chat_relation_id = chat_relation_id;
            this.unread_counts = unread_counts;
        }
    }
}
