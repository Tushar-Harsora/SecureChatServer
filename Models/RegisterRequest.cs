namespace SecureChatServer.Models
{
    public class RegisterEmailRequest
    {
        public string email { get; set; }
        public string? phone_number { get; set; }
        public string? username { get; set; }
        public string public_key { get; set; }

    }
}
