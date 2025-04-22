using RentACar.Core.Models.ML;
using RentACar.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Data.Models.User;
using RentACar.Infrastructure.Data;

namespace RentACar.Core.Services
{
    public class RentBillService : IRentBillService
    {
        private readonly ICarMetricsService _metricsService;
        private readonly IMaintenancePredictionService _predictionService;
        private readonly RentCarDbContext _context;

        public RentBillService(ICarMetricsService metricsService,
                             IMaintenancePredictionService predictionService,
                             RentCarDbContext context)
        {
            _metricsService = metricsService;
            _predictionService = predictionService;
            _context = context;
        }

        public async Task StartRental(int carId, string userId)
        {
            var car = await _context.Cars.FindAsync(carId);
            
            if (car == null)
                throw new ArgumentException("Car not found");

            var rentBill = new RentBill
            {
                CarId = carId,
                UserId = userId,
                DateOfTaking = DateTime.UtcNow,
                StartMileage = car.Mileage // Store initial mileage
            };

            // Initialize or update metrics at rental start
            await _metricsService.UpdateMetricsOnRentalStart(carId);
        }

        public async Task EndRental(int rentBillId)
        {
            var rentBill = await _context.Bills
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == rentBillId);

            if (rentBill == null)
                throw new ArgumentException("Rent bill not found");

            rentBill.EndMileage = rentBill.Car.Mileage;
            rentBill.DateOfReturn = DateTime.UtcNow;

            await _metricsService.UpdateMetricsOnRentalEnd(rentBill.CarId, rentBill.EndMileage.Value);
            await _context.SaveChangesAsync();

            var maintenanceData = await _metricsService.GetMaintenancePredictionData(rentBill.CarId);
            var prediction = await _predictionService.PredictMaintenanceAsync(maintenanceData);

            if (prediction.NeedsService)
            {
                await NotifyMaintenanceNeeded(rentBill.CarId, prediction);
            }
        }

        // Implementing the interface method
        public async Task EndRental(int rentBillId, int endMileage)
        {
            var rentBill = await _context.Bills
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == rentBillId);

            if (rentBill == null)
                throw new ArgumentException("Rent bill not found");

            rentBill.EndMileage = endMileage;
            rentBill.DateOfReturn = DateTime.UtcNow;

            await _metricsService.UpdateMetricsOnRentalEnd(rentBill.CarId, endMileage);
            await _context.SaveChangesAsync();

            var maintenanceData = await _metricsService.GetMaintenancePredictionData(rentBill.CarId);
            var prediction = await _predictionService.PredictMaintenanceAsync(maintenanceData);

            if (prediction.NeedsService)
            {
                await NotifyMaintenanceNeeded(rentBill.CarId, prediction);
            }
        }

        private async Task NotifyMaintenanceNeeded(int carId, MaintenancePrediction prediction)
        {
            // TODO: Implement notification logic
            // This could send emails, create maintenance tickets, etc.
            await Task.CompletedTask;
        }
    }
} 