using System.ComponentModel.DataAnnotations;
using RentACar.Core.Models.CategoryDto;
using RentACar.Infrastructure.Data;

namespace RentACar.Core.Models.CarDto
{
    public class CarViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(DataConstants.Car.ModelMaxLenght, MinimumLength = DataConstants.Car.ModelMinLenght)]
        public string Model { get; set; } = null!;

        [Required]
        [StringLength(DataConstants.Car.MakeMaxLenght, MinimumLength = DataConstants.Car.MakeMinLenght)]
        public string Make { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int Hp { get; set; }

        [Required]
        public bool IsRented { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        [Required]
        [Url]
        public string ImageUrl { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public int PricePerDay { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}
