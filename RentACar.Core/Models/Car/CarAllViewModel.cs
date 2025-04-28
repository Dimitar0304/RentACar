using System.Numerics;
using System.ComponentModel.DataAnnotations;
using RentACar.Infrastructure.Data;

namespace RentACar.Core.Models.CarDto
{
    public class CarAllViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(DataConstants.Car.MakeMaxLenght, MinimumLength = DataConstants.Car.MakeMinLenght)]
        public string Make { get; set; } = null!;

        [Required]
        [StringLength(DataConstants.Car.ModelMaxLenght, MinimumLength = DataConstants.Car.ModelMinLenght)]
        public string Model { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int Hp { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        [Required]
        [Url]
        public string ImageUrl { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = null!;

        [Required]
        public bool IsRented { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public int PricePerDay { get; set; }
    }
}
