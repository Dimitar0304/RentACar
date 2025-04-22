using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentACar.Infrastructure.Data.Models.Vehicle
{
    public class CarMetrics
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Car))]
        public int CarId { get; set; }

        public Car Car { get; set; } = null!;

        /// <summary>
        /// Last service date for the car
        /// </summary>
        [Required]
        public DateTime LastServiceDate { get; set; }

        /// <summary>
        /// Current engine temperature in Celsius
        /// </summary>
        [Required]
        public float EngineTemperature { get; set; }

        /// <summary>
        /// Oil level (0-1 scale)
        /// </summary>
        [Required]
        public float OilLevel { get; set; }

        /// <summary>
        /// Tire wear level (0-1 scale)
        /// </summary>
        [Required]
        public float TireWear { get; set; }

        /// <summary>
        /// Brake wear level (0-1 scale)
        /// </summary>
        [Required]
        public float BrakeWear { get; set; }

        /// <summary>
        /// Last time metrics were updated
        /// </summary>
        [Required]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Days since last service
        /// </summary>
        [NotMapped]
        public int DaysSinceLastService => (int)(DateTime.UtcNow - LastServiceDate).TotalDays;
    }
} 