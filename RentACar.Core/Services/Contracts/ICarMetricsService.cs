using RentACar.Core.Models.ML;
using RentACar.Infrastructure.Data.Models.Vehicle;

public interface ICarMetricsService
    {
        Task UpdateMetricsOnRentalStart(int carId);
        Task UpdateMetricsOnRentalEnd(int carId, int endMileage);
        Task<VehicleMaintenanceData> GetMaintenancePredictionData(int carId);
        Task<CarMetrics> GetCarMetrics(int carId);
    }