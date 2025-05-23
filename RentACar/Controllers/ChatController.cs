﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Services.Contracts;

namespace RentACar.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService chatService;

        public ChatController(IChatService _chatService)
        {
            chatService = _chatService;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
