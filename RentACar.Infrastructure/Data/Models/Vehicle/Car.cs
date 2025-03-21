using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RentACar.Infrastructure.Data.Models.Vehicle
{
    public class Car : IVehicle
    {
        public Car()
        {
            
        }

        /// <summary>
        /// Car identifier property
        /// </summary>
        [Key]
        [Comment("Car identifier")]
        public int Id { get; set; }

        /// <summary>
        /// Car model property
        /// </summary>
        [Required]
        [MinLength(DataConstants.Car.ModelMinLenght)]
        [MaxLength(DataConstants.Car.ModelMaxLenght)]
        [Comment("Car model")]
        public string Model { get; set; } = null!;

        /// <summary>
        /// Car horse power property
        /// </summary>
        [Required]
        [Comment("Car horse power")]
        public int Hp { get; set; }

        /// <summary>
        /// Car is rented property
        /// </summary>
        [Required]
        [Comment("Is rented")]
        public bool IsRented { get; set; }

        /// <summary>
        /// Car mileage
        /// </summary>
        [Required]
        [Comment("car mileage")]
        public int Mileage { get; set; }
    }
}
