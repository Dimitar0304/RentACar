using RentACar.Infrastructure.Data.Models.User;
using RentACar.Infrastructure.Data.Models.Vehicle;
using Microsoft.EntityFrameworkCore;

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
            if (await _context.Set<RentBill>().AnyAsync())
                return; // Data already seeded

            var cars = await _context.Cars.ToListAsync();
            var users = await _context.Users.ToListAsync();
            var random = new Random();

            if (!cars.Any() || !users.Any())
                return; // No cars or users to create rentals for

            var towns = new[] { "Sofia", "Plovdiv", "Varna", "Burgas", "Ruse" };
            var currentDate = DateTime.UtcNow;

            // Create a variety of rental scenarios
            foreach (var car in cars)
            {
                // Create completed rentals in the past
                for (int i = 0; i < random.Next(1, 4); i++) // 1-3 past rentals per car
                {
                    var user = users[random.Next(users.Count)];
                    var startDate = currentDate.AddDays(-random.Next(30, 180)); // Random start date in last 6 months
                    var duration = random.Next(1, 15); // 1-14 days rental
                    var startMileage = car.Mileage - random.Next(100, 5000); // Calculate past mileage
                    var drivenKm = random.Next(50, 1000);

                    var rentBill = new RentBill
                    {
                        CarId = car.Id,
                        UserId = user.Id,
                        TownOfRent = towns[random.Next(towns.Length)],
                        DateOfTaking = startDate,
                        DateOfReturn = startDate.AddDays(duration),
                        StartMileage = startMileage,
                        EndMileage = startMileage + drivenKm
                    };

                    await _context.AddAsync(rentBill);
                }

                // 30% chance of having an active rental
                if (random.NextDouble() < 0.3 && !car.IsRented)
                {
                    var user = users[random.Next(users.Count)];
                    var startDate = currentDate.AddDays(-random.Next(1, 7)); // Started within last week
                    var startMileage = car.Mileage;

                    var activeRental = new RentBill
                    {
                        CarId = car.Id,
                        UserId = user.Id,
                        TownOfRent = towns[random.Next(towns.Length)],
                        DateOfTaking = startDate,
                        StartMileage = startMileage,
                        // No end date or end mileage for active rentals
                    };

                    car.IsRented = true;
                    await _context.AddAsync(activeRental);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
} 