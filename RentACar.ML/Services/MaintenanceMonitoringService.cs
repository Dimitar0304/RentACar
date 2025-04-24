using RentACar.Infrastructure.Data.Models.Vehicle;
using RentACar.ML.Services.Interfaces;
using RentACar.ML.Services.Models;
using RentACar.ML.Services.Enums;
using Microsoft.EntityFrameworkCore;

namespace RentACar.ML.Services
{
    public class MaintenanceMonitoringService : IMaintenanceMonitoringService
    {
        private readonly RentCarDbContext _context;
        private const int SERVICE_INTERVAL_DAYS = 180; // 6 months
        private const float TIRE_WEAR_WARNING = 0.6f;
        private const float TIRE_WEAR_CRITICAL = 0.8f;
        private const float BRAKE_WEAR_WARNING = 0.6f;
        private const float BRAKE_WEAR_CRITICAL = 0.8f;
        private const float OIL_LEVEL_WARNING = 0.3f;
        private const float OIL_LEVEL_CRITICAL = 0.2f;
        private const float ENGINE_TEMP_WARNING = 95f;
        private const float ENGINE_TEMP_CRITICAL = 100f;

        public MaintenanceMonitoringService(RentCarDbContext context)
        {
            _context = context;
        }

        public async Task<List<MaintenanceAlert>> CheckMaintenanceStatusAsync()
        {
            var alerts = new List<MaintenanceAlert>();
            var cars = await _context.Cars
                .Include(c => c.CarMetrics.OrderByDescending(m => m.LastUpdated).Take(1))
                .ToListAsync();

            foreach (var car in cars)
            {
                var alert = await GetCarMaintenanceStatusAsync(car.Id);
                if (alert.Alerts.Any())
                {
                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public async Task<MaintenanceAlert> GetCarMaintenanceStatusAsync(int carId)
        {
            var car = await _context.Cars
                .Include(c => c.CarMetrics.OrderByDescending(m => m.LastUpdated).Take(1))
                .FirstOrDefaultAsync(c => c.Id == carId);

            if (car == null)
                throw new ArgumentException("Car not found", nameof(carId));

            var alert = new MaintenanceAlert
            {
                CarId = car.Id,
                CarModel = car.Model,
                Urgency = MaintenanceUrgency.Normal
            };

            var latestMetrics = car.CarMetrics.FirstOrDefault();
            if (latestMetrics == null)
                return alert;

            // Check service interval
            var daysSinceService = (DateTime.UtcNow - latestMetrics.LastServiceDate).TotalDays;
            if (daysSinceService >= SERVICE_INTERVAL_DAYS)
            {
                alert.Alerts.Add($"Regular maintenance required: {daysSinceService:F0} days since last service");
                UpdateUrgency(alert, MaintenanceUrgency.Warning);
            }

            // Check tire wear
            if (latestMetrics.TireWear >= TIRE_WEAR_CRITICAL)
            {
                alert.Alerts.Add($"Critical: Tires need immediate replacement ({latestMetrics.TireWear:P0} worn)");
                UpdateUrgency(alert, MaintenanceUrgency.Critical);
            }
            else if (latestMetrics.TireWear >= TIRE_WEAR_WARNING)
            {
                alert.Alerts.Add($"Warning: Tires showing significant wear ({latestMetrics.TireWear:P0})");
                UpdateUrgency(alert, MaintenanceUrgency.Warning);
            }

            // Check brake wear
            if (latestMetrics.BrakeWear >= BRAKE_WEAR_CRITICAL)
            {
                alert.Alerts.Add($"Critical: Brake pads need immediate replacement ({latestMetrics.BrakeWear:P0} worn)");
                UpdateUrgency(alert, MaintenanceUrgency.Critical);
            }
            else if (latestMetrics.BrakeWear >= BRAKE_WEAR_WARNING)
            {
                alert.Alerts.Add($"Warning: Brake pads showing significant wear ({latestMetrics.BrakeWear:P0})");
                UpdateUrgency(alert, MaintenanceUrgency.Warning);
            }

            // Check oil level
            if (latestMetrics.OilLevel <= OIL_LEVEL_CRITICAL)
            {
                alert.Alerts.Add($"Critical: Oil level critically low ({latestMetrics.OilLevel:P0})");
                UpdateUrgency(alert, MaintenanceUrgency.Critical);
            }
            else if (latestMetrics.OilLevel <= OIL_LEVEL_WARNING)
            {
                alert.Alerts.Add($"Warning: Oil level low ({latestMetrics.OilLevel:P0})");
                UpdateUrgency(alert, MaintenanceUrgency.Warning);
            }

            // Check engine temperature
            if (latestMetrics.EngineTemperature >= ENGINE_TEMP_CRITICAL)
            {
                alert.Alerts.Add($"Critical: Engine temperature too high ({latestMetrics.EngineTemperature:F1}°C)");
                UpdateUrgency(alert, MaintenanceUrgency.Critical);
            }
            else if (latestMetrics.EngineTemperature >= ENGINE_TEMP_WARNING)
            {
                alert.Alerts.Add($"Warning: Engine temperature elevated ({latestMetrics.EngineTemperature:F1}°C)");
                UpdateUrgency(alert, MaintenanceUrgency.Warning);
            }

            return alert;
        }

        private void UpdateUrgency(MaintenanceAlert alert, MaintenanceUrgency newUrgency)
        {
            if (newUrgency > alert.Urgency)
            {
                alert.Urgency = newUrgency;
            }
        }
    }
} 