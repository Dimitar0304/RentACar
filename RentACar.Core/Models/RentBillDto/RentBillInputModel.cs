namespace RentACar.Core.Models.RentBillDto
{
    public class RentBillInputModel
    {
        public int CarId { get; set; }
        public string UserId { get; set; } = null!;
        public string TownOfRent { get; set; } = null!;
        public decimal TotalPrice { get; set; }
    }
} 