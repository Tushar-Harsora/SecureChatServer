using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureChatServer.Helpers;
using SecureChatServer.Models;

namespace SecureChatServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        IChatService _chatservice;
        public ChatController(IChatService chatservice)
        {
            _chatservice = chatservice;
        }

        [Authorize]
        [HttpPost]
        [Route("getPreviouslyContacted")]
        public async Task<IActionResult> getPreviouslyContacted(int your_uid)
        {
            List<User> resp = await _chatservice.GetPreviouslyContacted(your_uid);

            return Ok(resp);
        }
    }
}
