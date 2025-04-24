using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Data.Models.Vehicle;
using RentACar.Infrastructure.Data.Models.User;

namespace RentACar.Infrastructure.Data.Seed
{
    public class RentBillSeeder : ISeeder
    {
        private readonly RentCarDbContext _context;

        public RentBillSeeder(RentCarDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Bills.AnyAsync())
            {
                return; // Data already seeded
            }

            var cars = await _context.Cars.ToListAsync();
            if (!cars.Any())
            {
                return; // No cars to create bills for
            }

            var users = await _context.Users.ToListAsync();
            if (!users.Any())
            {
                return; // No users to create bills for
            }

            var random = new Random();
            var currentDate = DateTime.UtcNow;

            // Create some completed rentals
            foreach (var car in cars.Take(3))
            {
                var user = users[random.Next(users.Count)];
                var startDate = currentDate.AddDays(-random.Next(1, 30));
                var endDate = startDate.AddDays(random.Next(1, 7));
                var startMileage = car.Mileage;
                var endMileage = startMileage + random.Next(100, 1000);

                var bill = new RentBill
                {
                    CarId = car.Id,
                    UserId = user.Id,
                    DateOfTaking = startDate,
                    DateOfReturn = endDate,
                    StartMileage = startMileage,
                    EndMileage = endMileage,
                    TotalPrice = random.Next(50, 500),
                    TownOfRent = "Sofia"
                };

                await _context.Bills.AddAsync(bill);
            }

            // Create some active rentals
            foreach (var car in cars.Skip(3).Take(2))
            {
                var user = users[random.Next(users.Count)];
                var startDate = currentDate.AddDays(-random.Next(1, 5));
                var startMileage = car.Mileage;

                var bill = new RentBill
                {
                    CarId = car.Id,
                    UserId = user.Id,
                    DateOfTaking = startDate,
                    StartMileage = startMileage,
                    TotalPrice = 0, // Will be calculated when rental ends
                    TownOfRent = "Sofia"
                };

                await _context.Bills.AddAsync(bill);
            }

            await _context.SaveChangesAsync();
        }
    }
} 