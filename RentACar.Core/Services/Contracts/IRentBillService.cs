using RentACar.Core.Models.RentBillDto;

namespace RentACar.Core.Services.Contracts
{
    public interface IRentBillService
    {
        Task<IEnumerable<RentBillViewModel>> GetAllRentBillsAsync();
        Task<IEnumerable<RentBillViewModel>> GetUserRentBillsAsync(string userId);
        Task<RentBillViewModel?> GetRentBillByIdAsync(int rentBillId);
        Task<bool> ReturnCarAsync(int rentBillId, int endMileage);
        Task<int> CreateRentBillAsync(RentBillInputModel model);
    }
}