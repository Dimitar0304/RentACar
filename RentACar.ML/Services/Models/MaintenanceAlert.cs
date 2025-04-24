using RentACar.ML.Services.Enums;

namespace RentACar.ML.Services.Models
{
    public class MaintenanceAlert
    {
        public int CarId { get; set; }
        public string CarModel { get; set; }
        public List<string> Alerts { get; set; } = new();
        public MaintenanceUrgency Urgency { get; set; }
    }
} 