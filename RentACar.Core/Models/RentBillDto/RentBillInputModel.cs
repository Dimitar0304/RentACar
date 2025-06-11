using System.ComponentModel.DataAnnotations;

namespace RentACar.Core.Models.RentBillDto
{
    public class RentBillInputModel
    {
        public int CarId { get; set; }

        public string UserId { get; set; } = null!;

        [Required]
        public string TownOfRent { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfTaking { get; set; }

        public DateTime? DateOfReturn { get; set; }

        public decimal TotalPrice { get; set; }
        public int StartMileage { get; set; }
    }
} 