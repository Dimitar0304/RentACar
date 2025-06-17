using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Services.Contracts;
using RentACar.Core.Models.Chat;

namespace RentACar.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService chatService;

        public ChatController(IChatService _chatService)
        {
            chatService = _chatService;
        }

        [Authorize(Roles = "User")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SendMessage(MessageInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            await chatService.SendMessage(model);

            TempData["MessageSent"] = "Your message has been sent to support.";

            return RedirectToAction("Index");
        }
    }
}
