namespace RentACar.Core.Models.RentBillDto
{
    public class RentBillViewModel
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string CarMake { get; set; } = null!;
        public string CarModel { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserFirstName { get; set; } = null!;
        public string UserLastName { get; set; } = null!;
        public DateTime DateOfTaking { get; set; }
        public DateTime? DateOfReturn { get; set; }
        public string TownOfRent { get; set; } = null!;
        public int StartMileage { get; set; }
        public int? EndMileage { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 