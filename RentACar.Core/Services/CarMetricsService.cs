using Microsoft.EntityFrameworkCore;
using RentACar.Core.Models.ML;
using RentACar.Infrastructure.Data;
using RentACar.Infrastructure.Data.Models.Vehicle;

namespace RentACar.Core.Services
{
    public class CarMetricsService : ICarMetricsService
    {
        private readonly RentCarDbContext _context;
        private const float TIRE_WEAR_RATE = 0.0001f; // Wear per kilometer
        private const float BRAKE_WEAR_RATE = 0.00015f; // Wear per kilometer
        private const float OIL_CONSUMPTION_RATE = 0.00005f; // Oil consumption per kilometer

        public CarMetricsService(RentCarDbContext context)
        {
            _context = context;
        }

        public async Task UpdateMetricsOnRentalStart(int carId)
        {
            var car = await _context.Cars
                .Include(c => c.Metrics)
                .FirstOrDefaultAsync(c => c.Id == carId);

            if (car == null)
                throw new ArgumentException("Car not found");

            if (car.Metrics == null)
            {
                // Initialize metrics if they don't exist
                car.Metrics = new CarMetrics
                {
                    CarId = carId,
                    LastServiceDate = DateTime.UtcNow,
                    EngineTemperature = 85, // Normal operating temperature
                    OilLevel = 1.0f, // Full oil
                    TireWear = 0, // New tires
                    BrakeWear = 0, // New brakes
                    LastUpdated = DateTime.UtcNow
                };
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateMetricsOnRentalEnd(int carId, int endMileage)
        {
            var car = await _context.Cars
                .Include(c => c.Metrics)
                .FirstOrDefaultAsync(c => c.Id == carId);

            if (car == null || car.Metrics == null)
                throw new ArgumentException("Car not found");

            var distanceDriven = endMileage - car.Mileage;
            car.Mileage = endMileage;

            // Update wear metrics
            car.Metrics.TireWear += distanceDriven * TIRE_WEAR_RATE;
            car.Metrics.BrakeWear += distanceDriven * BRAKE_WEAR_RATE;
            car.Metrics.OilLevel -= distanceDriven * OIL_CONSUMPTION_RATE;

            // Simulate engine temperature variation (85-95 degrees Celsius)
            car.Metrics.EngineTemperature = 85 + (float)(new Random().NextDouble() * 10);

            // Ensure values stay within bounds
            car.Metrics.TireWear = Math.Min(1.0f, car.Metrics.TireWear);
            car.Metrics.BrakeWear = Math.Min(1.0f, car.Metrics.BrakeWear);
            car.Metrics.OilLevel = Math.Max(0.0f, car.Metrics.OilLevel);
            car.Metrics.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<VehicleMaintenanceData> GetMaintenancePredictionData(int carId)
        {
            var car = await _context.Cars
                .Include(c => c.Metrics)
                .FirstOrDefaultAsync(c => c.Id == carId);

            if (car == null || car.Metrics == null)
                throw new ArgumentException("Car or metrics not found");

            return new VehicleMaintenanceData
            {
                Mileage = car.Mileage,
                DaysSinceLastService = car.Metrics.DaysSinceLastService,
                EngineTemperature = car.Metrics.EngineTemperature,
                OilLevel = car.Metrics.OilLevel,
                TireWear = car.Metrics.TireWear,
                BrakeWear = car.Metrics.BrakeWear
            };
        }

        public async Task<CarMetrics> GetCarMetrics(int carId)
        {
            var metrics = await _context.Set<CarMetrics>()
                .FirstOrDefaultAsync(m => m.CarId == carId);

            if (metrics == null)
                throw new ArgumentException("Metrics not found for car");

            return metrics;
        }
    }
} 