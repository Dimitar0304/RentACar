using RentACar.Core.Models.ML;

namespace RentACar.Core.Services.Interfaces
{
    public interface IMaintenancePredictionService
    {
        Task<MaintenancePrediction> PredictMaintenanceAsync(VehicleMaintenanceData data);
        Task TrainModelAsync(IEnumerable<VehicleMaintenanceData> trainingData);
    }
} 