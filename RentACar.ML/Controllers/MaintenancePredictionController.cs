using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Models.ML;
using RentACar.ML.Services.Interfaces;
using Microsoft.ML;

namespace RentACar.ML.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenancePredictionController : ControllerBase
    {
        private readonly IMaintenancePredictionService _predictionService;
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public MaintenancePredictionController(IMaintenancePredictionService predictionService)
        {
            _predictionService = predictionService;
            _mlContext = new MLContext();
        }

        [HttpPost("predict")]
        public async Task<ActionResult<MaintenancePrediction>> PredictMaintenance([FromBody] VehicleMaintenanceData data)
        {
            try
            {
                var prediction = await _predictionService.PredictMaintenanceAsync(data);
                return Ok(prediction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error predicting maintenance: {ex.Message}");
            }
        }

        [HttpPost("train")]
        public async Task<IActionResult> TrainModel([FromBody] List<VehicleMaintenanceData> trainingData)
        {
            try
            {
                await _predictionService.TrainModelAsync(trainingData);
                return Ok("Model trained successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error training model: {ex.Message}");
            }
        }

        private async Task TrainModelAsync(IEnumerable<VehicleMaintenanceData> trainingData)
        {
            // 1. Load training data
            var trainingDataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            // 2. Create ML pipeline
            var pipeline = _mlContext.Transforms.Concatenate("Features",
                nameof(VehicleMaintenanceData.Mileage),
                nameof(VehicleMaintenanceData.DaysSinceLastService),
                // ... other features ...
            )
            .Append(_mlContext.BinaryClassification.Trainers.FastTree(
                labelColumnName: nameof(VehicleMaintenanceData.NeedsService)
            ));

            // 3. Train the model
            _model = pipeline.Fit(trainingDataView);
        }

        private float CalculateEngineHealth(VehicleMaintenanceData data)
        {
            // Engine health based on temperature and oil level
            return (100 - (data.EngineTemperature / 150 * 100) * 0.6f) + 
                   (data.OilLevel * 100 * 0.4f);
        }

        private float CalculateTireHealth(VehicleMaintenanceData data)
        {
            // Tire health based on wear level
            return 100 - (data.TireWear * 100);
        }

        private float CalculateBrakeHealth(VehicleMaintenanceData data)
        {
            // Implementation of CalculateBrakeHealth method
            return 0f; // Placeholder return, actual implementation needed
        }
    }
} 