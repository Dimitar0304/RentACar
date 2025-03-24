
using RentACar.Core.Models.CarDto;
using RentACar.Core.Models.CategoryDto;

namespace RentACar.Core.Services.Contracts
{
    public interface ICarService
    {
        Task AddCarAsync(CarViewModel car);
        Task UpdateCarAsync(CarViewModel car);
        Task DeleteCarAsync(int carId);
        Task<CarViewModel> GetCarByIdAsync(int carId);
        Task<IEnumerable<CarAllViewModel>> GetAllCarsAsync();
        Task<IEnumerable<CarBrandViewModel>> GetAllCarBrandsAsync();
        Task<IEnumerable<CategoryViewModel>> GetAllCategories();
        bool IsCarExistInDb(CarViewModel car);

    }
}
