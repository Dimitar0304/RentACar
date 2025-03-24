using RentACar.Core.Models.CarDto;
using RentACar.Core.Models.CategoryDto;
using RentACar.Core.Services.Contracts;
using RentACar.Infrastructure.Data;
using RentACar.Infrastructure.Data.Models.Vehicle;

namespace RentACar.Core.Services.CarDto
{
    class CarService : ICarService
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
                   Id=car.Id,
                   Model = car.Model,
                   Make = car.Make,
                   Category= new Category()
                   {
                       Id = car.CategoryId,
                       Name = car.Category.Name
                   },
                   CategoryId = car.CategoryId,
                   Hp = car.Hp,
                   IsRented = car.IsRented,
                   Mileage = car.Mileage
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

        public Task<IEnumerable<CategoryViewModel>> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public Task<CarViewModel> GetCarByIdAsync(int carId)
        {
            throw new NotImplementedException();
        }

        public bool IsCarExistInDb(CarViewModel car)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCarAsync(CarViewModel car)
        {
            throw new NotImplementedException();
        }
    }
}
