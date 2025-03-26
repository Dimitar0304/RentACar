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
        public int Id { get; set; }

        /// <summary>
        /// Car make
        /// </summary>
        [Required]
        [MinLength(DataConstants.Car.MakeMinLenght)]
        [MaxLength(DataConstants.Car.MakeMaxLenght)]
        public string Make { get; set; } = null!;

        /// <summary>
        /// Car model property
        /// </summary>
        [Required]
        [MinLength(DataConstants.Car.ModelMinLenght)]
        [MaxLength(DataConstants.Car.ModelMaxLenght)]
        public string Model { get; set; } = null!;

        /// <summary>
        /// Car horse power property
        /// </summary>
        [Required]
        public int Hp { get; set; }

        /// <summary>
        /// Car is rented property
        /// </summary>
        [Required]
        public bool IsRented { get; set; }

        /// <summary>
        /// Car mileage
        /// </summary>
        [Required]
        public int Mileage { get; set; }

        /// <summary>
        /// Car category identifier
        /// </summary>
        [Required]
        public int CategoryId { get; set; }

        /// <summary>
        /// Car category
        /// </summary>
        [Required]
        public Category Category { get; set; }

        /// <summary>
        /// Car image
        /// </summary>
        [Required]
        public string ImageUrl { get; set; } = null!;
    }
}
