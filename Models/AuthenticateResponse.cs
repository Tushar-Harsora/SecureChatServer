namespace SecureChatServer.Models
{
    public class AuthenticateResponse
    {
        public string email {  get; set; }
        public string phone_number { get; set; }
        public string username { get; set;  }
        public string Token { get; set; }
    }
}
