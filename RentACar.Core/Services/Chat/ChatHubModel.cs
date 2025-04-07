
using Microsoft.AspNetCore.SignalR;
using RentACar.Core.Services.Contracts;
using Microsoft.AspNetCore.ResponseCompression;

namespace RentACar.Core.Services.Chat
{
    public class ChatHubModel : Hub,IChatService
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
