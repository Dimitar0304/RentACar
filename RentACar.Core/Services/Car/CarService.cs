using Microsoft.EntityFrameworkCore;
using RentACar.Core.Models.CarDto;
using RentACar.Core.Models.CategoryDto;
using RentACar.Core.Services.Contracts;
using RentACar.Infrastructure.Data;
using RentACar.Infrastructure.Data.Models.Vehicle;

namespace RentACar.Core.Services.CarDto
{
    public class CarService : ICarService
    {
        private readonly RentCarDbContext dbContext;
        public CarService(RentCarDbContext context)
        {
            dbContext = context;
        }
        public async Task AddCarAsync(CarViewModel car)
        {
            if (car!=null)
            {
                var c = new Car()
                {
                   Model = car.Model,
                   Make = car.Make,
                   CategoryId = car.CategoryId,
                   Hp = car.Hp,
                   IsRented =false,
                   Mileage = car.Mileage,
                   ImageUrl=car.ImageUrl
                };

              await dbContext.AddAsync(c);
              await dbContext.SaveChangesAsync();
            }
        }

        public Task DeleteCarAsync(int carId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CarBrandViewModel>> GetAllCarBrandsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CarAllViewModel>> GetAllCarsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategories()
        {
            return await dbContext.Categories.Select(c => new CategoryViewModel()
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
        }

        public Task<CarViewModel> GetCarByIdAsync(int carId)
        {
            throw new NotImplementedException();
        }

        public bool IsCarExistInDb(CarViewModel car)
        {
            if (dbContext.Cars.FirstOrDefault(c=>c.Id ==car.Id)!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task UpdateCarAsync(CarViewModel car)
        {
            throw new NotImplementedException();
        }
    }
}
