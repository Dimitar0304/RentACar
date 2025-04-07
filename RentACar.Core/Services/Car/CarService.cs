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

        public async Task<IEnumerable<CarAllViewModel>> GetAllCarsAsync()
        {
            if (await dbContext.Cars.ToListAsync()!=null)
            {
                var cars = await dbContext.Cars
                    .Select(c => new CarAllViewModel()
                    {
                        Id=c.Id,
                        Make = c.Make,
                        Model = c.Model,
                        Category = c.Category.Name,
                        Hp = c.Hp,
                        ImageUrl = c.ImageUrl,
                        IsRented = c.IsRented,
                        Mileage = c.Mileage
                    })
                    .ToListAsync();
                return  cars;
            }
            else
            {
                return new List<CarAllViewModel>();
            }
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategories()
        {
            return await dbContext.Categories.Select(c => new CategoryViewModel()
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
        }

        public async Task<CarViewModel> GetCarByIdAsync(int carId)
        {
            return await dbContext.Cars
                 .Select(c => new CarViewModel
                 {
                     Id = carId,
                     Make = c.Make,
                     Model = c.Model,
                     CategoryId = c.CategoryId,
                     Hp = c.Hp,
                     ImageUrl = c.ImageUrl,
                     IsRented = c.IsRented,
                     Mileage = c.Mileage
                 })
                 .FirstOrDefaultAsync(c => c.Id == carId);
                
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

        public async Task UpdateCarAsync(CarViewModel car)
        {
            Car carToEdit = await dbContext.Cars.FirstOrDefaultAsync(c => c.Id == car.Id);
            if (carToEdit!=null)
            {
                carToEdit.Make = car.Make;
                carToEdit.Model = car.Model;
                carToEdit.Hp = car.Hp;
                carToEdit.IsRented = car.IsRented;
                carToEdit.CategoryId = car.CategoryId;
                carToEdit.ImageUrl = car.ImageUrl;
                carToEdit.Mileage = car.Mileage;
            }
           dbContext.Cars.Update(carToEdit);
          await dbContext.SaveChangesAsync();
        }
    }
}
