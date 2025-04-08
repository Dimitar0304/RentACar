using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Services.Contracts;

namespace RentACar.Controllers
{
    public class ChatController:Controller
    {
        private readonly IChatService chatService;

        public ChatController(IChatService _chatService)
        {
            chatService = _chatService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
