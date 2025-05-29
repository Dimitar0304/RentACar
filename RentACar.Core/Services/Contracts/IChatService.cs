using RentACar.Core.Models.Chat;

namespace RentACar.Core.Services.Contracts
{
    public interface IChatService
    {
        Task SendMessage(MessageInputModel messageModel);
        Task UpdateMessageStatus(MessageStatusUpdateModel statusModel);
        Task MarkMessageAsRead(MessageReadUpdateModel readModel);
        Task GetUnreadMessages(string userId);
    }
}