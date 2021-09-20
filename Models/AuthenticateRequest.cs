using System.ComponentModel.DataAnnotations;

namespace SecureChatServer.Models
{
    public class AuthenticateEmailRequest
    {
        [Required]
        public string Email {  get; set; }

        [Required]
        public string signedText {  get; set; }
    }
}
