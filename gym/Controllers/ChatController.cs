using gym.Services.Chatbot;
using Microsoft.AspNetCore.Mvc;

namespace gym.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatBotService _chatBotService;

        public ChatController(IChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Ask(string message)
        {
            var reply = await _chatBotService.GetAnswerAsync(message);
            return Json(new { response = reply });
        }
    }
}
