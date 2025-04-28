using RentACar.Infrastructure.Data.Models.Interfaces;
using RentACar.Infrastructure.Data.Models.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentACar.Infrastructure.Data.Models.Vehicle
{
    public class Car : IVehicle
    {
        public Car()
        {
            RentBills = new List<RentBill>();
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
        public required string Make { get; set; }

        /// <summary>
        /// Car model property
        /// </summary>
        [Required]
        [MinLength(DataConstants.Car.ModelMinLenght)]
        [MaxLength(DataConstants.Car.ModelMaxLenght)]
        public required string Model { get; set; }

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
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        /// <summary>
        /// Car category
        /// </summary>
        [Required]
        public required Category Category { get; set; }

        /// <summary>
        /// Car image
        /// </summary>
        [Required]
        public required string ImageUrl { get; set; }

        /// <summary>
        /// Car price per day
        /// </summary>
        [Required]
        public int PricePerDay { get; set; }

        /// <summary>
        /// Navigation property for rent bills
        /// </summary>
        public ICollection<RentBill> RentBills { get; set; }
    }
}
