using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureChatServer.Helpers;
using SecureChatServer.Models;

namespace SecureChatServer.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        IChatService _chatservice;
        public ChatController(IChatService chatservice)
        {
            _chatservice = chatservice;
        }

        [HttpPost]
        [Route("getPreviouslyContacted")]
        public async Task<IActionResult> getPreviouslyContacted()
        {
            int client_id = GetCurrentUserId();
            List<PreviouslyContactedUser> resp = await _chatservice.GetPreviouslyContacted(client_id);

            return Ok(resp);
        }

        [HttpPost]
        [Route("getConversation")]
        public async Task<IActionResult> getConversation(GetConversationRequest request)
        {
            int chat_relation = Int32.Parse(request.chat_relation_id);
            int client_id = GetCurrentUserId();

            bool valid_request = await _chatservice.CheckValidChatRelationId(client_id, chat_relation);
            if (!valid_request)
                return Unauthorized("Chat Relation Id is not Yours");
            List<Message> messages = await _chatservice.GetConversation(chat_relation, client_id);
            messages.Sort(delegate (Message a, Message b)
            {
                return a.message_at.CompareTo(b.message_at);
            });

            return Ok(messages);
        }

        [HttpPost]
        [Route("initializeChat")]
        public async Task<IActionResult> initializeChat(int dest_uid)
        {
            int client_id = GetCurrentUserId();
            await _chatservice.InitializeChat(client_id, dest_uid);

            return Ok();
        }

        [HttpPost]
        [Route("sendMessage")]
        public async Task<IActionResult> sendMessage(Message message)
        {
            if (message.sender_id != GetCurrentUserId())
                return Unauthorized("sender_id should be your id");

            try
            {
                int message_id = await _chatservice.SendMessage(message);
                return Ok(message_id);
            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private int GetCurrentUserId()
        {
            object? userObject = HttpContext.Items["User"];
            if (userObject == null)
            {
                throw new Exception();
            }
            int client_id = ((User)userObject).uid;

            return client_id;
        }
    }
}
