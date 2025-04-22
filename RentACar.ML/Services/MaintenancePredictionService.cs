using Microsoft.ML;
using Microsoft.ML.Data;
using RentACar.Core.Models.ML;
using RentACar.ML.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RentACar.ML.Services
{
    public class MaintenancePredictionService : IMaintenancePredictionService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private readonly string _modelPath = "maintenance_model.zip";

        public MaintenancePredictionService()
        {
            _mlContext = new MLContext(seed: 1);
        }

        public async Task TrainModelAsync(IEnumerable<VehicleMaintenanceData> trainingData)
        {
            var trainingDataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.Concatenate("Features",
                nameof(VehicleMaintenanceData.Mileage),
                nameof(VehicleMaintenanceData.DaysSinceLastService),
                nameof(VehicleMaintenanceData.EngineTemperature),
                nameof(VehicleMaintenanceData.OilLevel),
                nameof(VehicleMaintenanceData.TireWear),
                nameof(VehicleMaintenanceData.BrakeWear))
                .Append(_mlContext.BinaryClassification.Trainers.FastTree(
                    labelColumnName: nameof(VehicleMaintenanceData.NeedsService),
                    numberOfLeaves: 20,
                    numberOfTrees: 100,
                    minimumExampleCountPerLeaf: 10));

            _model = pipeline.Fit(trainingDataView);
            await SaveModelAsync();
        }

        public async Task<MaintenancePrediction> PredictMaintenanceAsync(VehicleMaintenanceData data)
        {
            if (_model == null)
            {
                await LoadModelAsync();
            }

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<VehicleMaintenanceData, MaintenancePrediction>(_model);
            var prediction = predictionEngine.Predict(data);

            // Calculate component health scores based on the input data
            prediction.EngineHealthScore = CalculateEngineHealth(data);
            prediction.TireHealthScore = CalculateTireHealth(data);
            prediction.BrakeHealthScore = CalculateBrakeHealth(data);
            prediction.PredictedDaysUntilService = CalculateDaysUntilService(data);

            return prediction;
        }

        private float CalculateEngineHealth(VehicleMaintenanceData data)
        {
            // Simple weighted calculation based on engine temperature and oil level
            return (100 - (data.EngineTemperature / 150 * 100) * 0.6f) + 
                   (data.OilLevel * 100 * 0.4f);
        }

        private float CalculateTireHealth(VehicleMaintenanceData data)
        {
            // Convert tire wear to health percentage (100 - wear percentage)
            return 100 - (data.TireWear * 100);
        }

        private float CalculateBrakeHealth(VehicleMaintenanceData data)
        {
            // Convert brake wear to health percentage (100 - wear percentage)
            return 100 - (data.BrakeWear * 100);
        }

        private int CalculateDaysUntilService(VehicleMaintenanceData data)
        {
            // Calculate based on current metrics and standard service intervals
            const int standardServiceInterval = 180; // 6 months in days
            return (int)(standardServiceInterval * (1 - Math.Max(
                data.TireWear,
                Math.Max(data.BrakeWear, 1 - data.OilLevel)
            )));
        }

        private async Task SaveModelAsync()
        {
            await Task.Run(() => _mlContext.Model.Save(_model, null, _modelPath));
        }

        private async Task LoadModelAsync()
        {
            if (File.Exists(_modelPath))
            {
                await Task.Run(() => _model = _mlContext.Model.Load(_modelPath, out var _));
            }
            else
            {
                throw new FileNotFoundException("Model file not found. Please train the model first.");
            }
        }
    }
}