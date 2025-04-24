using RentACar.Infrastructure.Data.Models.Vehicle;
using Microsoft.EntityFrameworkCore;

namespace RentACar.Infrastructure.Data.Seed
{
    public class CarMetricsSeeder : ISeeder
    {
        private readonly RentCarDbContext _context;

        public CarMetricsSeeder(RentCarDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Set<CarMetrics>().AnyAsync())
                return; // Data already seeded

            var cars = await _context.Cars.ToListAsync();
            var random = new Random();
            var currentDate = DateTime.UtcNow;

            foreach (var car in cars)
            {
                // Create historical metrics (last 6 months of data)
                for (int i = 0; i < 6; i++)
                {
                    var metrics = new CarMetrics
                    {
                        CarId = car.Id,
                        LastServiceDate = currentDate.AddDays(-random.Next(i * 30, (i + 1) * 30)),
                        EngineTemperature = 85 + (float)(random.NextDouble() * 10), // 85-95°C
                        OilLevel = Math.Max(0.2f, 1.0f - (i * 0.1f) + (float)(random.NextDouble() * 0.2)), // Decreasing oil level over time
                        TireWear = Math.Min(1.0f, (i * 0.1f) + (float)(random.NextDouble() * 0.1)), // Increasing tire wear over time
                        BrakeWear = Math.Min(1.0f, (i * 0.08f) + (float)(random.NextDouble() * 0.1)), // Increasing brake wear over time
                        LastUpdated = currentDate.AddDays(-i * 30)
                    };

                    await _context.AddAsync(metrics);
                }

                // Add current metrics
                var currentMetrics = new CarMetrics
                {
                    CarId = car.Id,
                    LastServiceDate = currentDate.AddDays(-random.Next(1, 30)),
                    EngineTemperature = 87 + (float)(random.NextDouble() * 5), // Normal operating temperature
                    OilLevel = 0.8f + (float)(random.NextDouble() * 0.2), // 80-100% after recent service
                    TireWear = 0.1f + (float)(random.NextDouble() * 0.2), // 10-30% wear
                    BrakeWear = 0.1f + (float)(random.NextDouble() * 0.15), // 10-25% wear
                    LastUpdated = currentDate
                };

                await _context.AddAsync(currentMetrics);
            }

            await _context.SaveChangesAsync();
        }
    }
} 