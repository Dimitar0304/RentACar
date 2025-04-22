using RentACar.Core.Models.ML;

namespace RentACar.ML.Services.Interfaces
{
    public interface IMaintenancePredictionService
    {
        /// <summary>
        /// Predicts maintenance needs based on vehicle data
        /// </summary>
        /// <param name="data">Vehicle maintenance data</param>
        /// <returns>Maintenance prediction results</returns>
        Task<MaintenancePrediction> PredictMaintenanceAsync(VehicleMaintenanceData data);

        /// <summary>
        /// Trains the ML model using historical maintenance data
        /// </summary>
        /// <param name="trainingData">Collection of historical vehicle maintenance data</param>
        Task TrainModelAsync(IEnumerable<VehicleMaintenanceData> trainingData);
    }
} 