
namespace RentACar.Core.Services.Contracts
{
    public interface IChatService
    {
        public Task SendMessage(string user, string message);
    }
}
