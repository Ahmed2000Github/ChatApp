using ChatAppCore.DTOs;
using ChatAppServer.Hubs;
using ChatAppServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatAppServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class HomeController : ControllerBase
    {
        private readonly IHomeServices _homeServices;

        public HomeController(IHomeServices homeServices)
        {
            _homeServices = homeServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetContactConversations()
        {
            return Ok(await _homeServices.GetAllUserContactConversations(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersToContact()
        {
            return Ok(await _homeServices.GetAllUsersToContact(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetMessages(string conversationId)
        {
            return Ok(await _homeServices.GetAllUserMessagesInConversation(User.FindFirstValue(ClaimTypes.NameIdentifier),conversationId));
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage([FromBody] MessageFormDTO message)
        {
            var result = await _homeServices.AddMessageToConversation(message);

            if (result.okResult != null)
            {
                return Ok(result.okResult);
            }
            if (result.badRequestResult != null)
            {
                return BadRequest(result.badRequestResult);
            }
            return StatusCode(500, result.internalErrorResult);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateMessage([FromBody] string messageId)
        {
            var result = await _homeServices.UpdateMessageToReaded(messageId);
                return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddConversation([FromBody] string recieverId)
        {
            return Ok(await _homeServices.AddNewConversation(User.FindFirstValue(ClaimTypes.NameIdentifier), recieverId));
        }

        [HttpDelete("{conversationId}")]
        public async Task<IActionResult> DeleteConversationById(string conversationId)
        {
            return Ok(await _homeServices.DeleteConversationById(User.FindFirstValue(ClaimTypes.NameIdentifier), conversationId));
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] MessageFileFormDTO message)
        {
            var result = await _homeServices.AddMessageFileToConversation(message);

            if (result.okResult != null)
            { return Ok(result.okResult);
            }
            if (result.badRequestResult != null)
            {
                return BadRequest(result.badRequestResult);
            }
            return StatusCode(500, result.internalErrorResult);
        }

    }
}
