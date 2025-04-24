using RentACar.ML.Services.Models;

namespace RentACar.ML.Services.Interfaces
{
    public interface IMaintenanceMonitoringService
    {
        Task<List<MaintenanceAlert>> CheckMaintenanceStatusAsync();
        Task<MaintenanceAlert> GetCarMaintenanceStatusAsync(int carId);
    }
} 