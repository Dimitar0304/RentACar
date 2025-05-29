using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using RentACar.Core.Services.Contracts;
using RentACar.Infrastructure.Data;
using RentACar.Infrastructure.Data.Models.User;
using RentACar.Core.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace RentACar.Core.Services.Chat
{
    public class ChatHubModel : Hub, IChatService
    {
        private readonly RentCarDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHubModel(RentCarDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SendMessage(MessageInputModel messageModel)
        {
            var user = await _userManager.FindByIdAsync(messageModel.SenderId);
            if (user == null) return;

            var message = new Message
            {
                Content = messageModel.Content,
                SenderId = messageModel.SenderId,
                SenderName = messageModel.SenderName,
                Timestamp = DateTime.UtcNow,
                IsRead = false,
                Status = "Open"
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            // Get all support users
            var supportUsers = await _userManager.GetUsersInRoleAsync("Support");
            
            // Send to all support users
            foreach (var supportUser in supportUsers)
            {
                await Clients.User(supportUser.Id).SendAsync("ReceiveMessage", 
                    new MessageViewModel
                    {
                        Id = message.Id,
                        Content = message.Content,
                        SenderId = message.SenderId,
                        SenderName = message.SenderName,
                        Timestamp = message.Timestamp,
                        IsRead = message.IsRead,
                        Status = message.Status
                    });
            }
        }

        public async Task UpdateMessageStatus(MessageStatusUpdateModel statusModel)
        {
            var message = await _context.Messages.FindAsync(statusModel.MessageId);
            if (message == null) return;

            message.Status = statusModel.Status;
            await _context.SaveChangesAsync();

            // Notify the original sender about the status update
            await Clients.User(message.SenderId).SendAsync("MessageStatusUpdated", 
                new MessageViewModel
                {
                    Id = message.Id,
                    Status = message.Status
                });
        }

        public async Task MarkMessageAsRead(MessageReadUpdateModel readModel)
        {
            var message = await _context.Messages.FindAsync(readModel.MessageId);
            if (message == null) return;

            message.IsRead = readModel.IsRead;
            await _context.SaveChangesAsync();

            // Notify the original sender about the read status
            await Clients.User(message.SenderId).SendAsync("MessageReadStatusUpdated", 
                new MessageViewModel
                {
                    Id = message.Id,
                    IsRead = message.IsRead
                });
        }

        public async Task GetUnreadMessages(string userId)
        {
            var unreadMessages = await _context.Messages
                .Where(m => m.SenderId == userId && !m.IsRead)
                .Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.SenderId,
                    SenderName = m.SenderName,
                    Timestamp = m.Timestamp,
                    IsRead = m.IsRead,
                    Status = m.Status
                })
                .ToListAsync();

            await Clients.Caller.SendAsync("ReceiveUnreadMessages", unreadMessages);
        }

    }
}
