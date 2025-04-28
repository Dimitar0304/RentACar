using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Data.Models.Vehicle;
using RentACar.Infrastructure.Data.Models.User;

namespace RentACar.Infrastructure.Data.Seed
{
    public class CarSeeder : ISeeder
    {
        private readonly RentCarDbContext _context;

        public CarSeeder(RentCarDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (_context.Cars.Any())
            {
                return;
            }

            var categories = await _context.Categories.ToListAsync();

            await _context.Cars.AddRangeAsync(new[]
            {
                new Car { 
                    Make = "Toyota", 
                    Model = "Camry", 
                    CategoryId = 1,
                    Category = categories.First(c => c.Id == 1),
                    ImageUrl = "toyota-camry.jpg",
                    Hp = 180,
                    IsRented = false,
                    Mileage = 0,
                    PricePerDay = 50,
                    RentBills = new List<RentBill>()
                },
                new Car { 
                    Make = "Honda", 
                    Model = "Civic", 
                    CategoryId = 1,
                    Category = categories.First(c => c.Id == 1),
                    ImageUrl = "honda-civic.jpg",
                    Hp = 150,
                    IsRented = false,
                    Mileage = 0,
                    PricePerDay = 45,
                    RentBills = new List<RentBill>()
                },
                new Car { 
                    Make = "BMW", 
                    Model = "X5", 
                    CategoryId = 2,
                    Category = categories.First(c => c.Id == 2),
                    ImageUrl = "bmw-x5.jpg",
                    Hp = 300,
                    IsRented = false,
                    Mileage = 0,
                    PricePerDay = 80,
                    RentBills = new List<RentBill>()
                }
            });

            await _context.SaveChangesAsync();
        }
    }
} 