using RentACar.Core.Models.CategoryDto;

namespace RentACar.Core.Models.CarDto
{
    public class CarViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; } = null!;
        public string Make { get; set; } = null!;
        public int Hp { get; set; }
        public bool IsRented { get; set; }
        public int CategoryId { get; set; }
        public CategoryViewModel Category { get; set; } = null!;
        public int Mileage { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; } = null!;
    }
}
