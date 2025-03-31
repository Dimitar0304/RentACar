using System.Numerics;

namespace RentACar.Core.Models.CarDto
{
    public class CarAllViewModel
    {
        public string Make { get; set; } = null!;

        public string Model { get; set; } = null!;

        public int Hp { get; set; }
        public int Mileage { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string Category { get; set; } = null!;

        public bool IsRented { get; set; }

    }
}
