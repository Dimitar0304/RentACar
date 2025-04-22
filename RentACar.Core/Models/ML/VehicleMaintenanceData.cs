using Microsoft.ML.Data;

namespace RentACar.Core.Models.ML
{
    public class VehicleMaintenanceData
    {
        [LoadColumn(0)]
        public float Mileage { get; set; }

        [LoadColumn(1)]
        public float DaysSinceLastService { get; set; }

        [LoadColumn(2)]
        public float EngineTemperature { get; set; }

        [LoadColumn(3)]
        public float OilLevel { get; set; }

        [LoadColumn(4)]
        public float TireWear { get; set; }

        [LoadColumn(5)]
        public float BrakeWear { get; set; }

        [LoadColumn(6)]
        public bool NeedsService { get; set; }
    }

    public class MaintenancePrediction
    {
        [ColumnName("PredictedLabel")]
        public bool NeedsService { get; set; }

        [ColumnName("Probability")]
        public float Probability { get; set; }

        public float EngineHealthScore { get; set; }
        public float TireHealthScore { get; set; }
        public float BrakeHealthScore { get; set; }
        public int PredictedDaysUntilService { get; set; }
    }
} 