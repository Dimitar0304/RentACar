using System.ComponentModel.DataAnnotations;
using RentACar.Infrastructure.Data;

namespace RentACar.Core.Models.RentBillDto
{
    public class RentBillInputModel
    {
        [Required]
        public int CarId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [StringLength(DataConstants.RentBill.TownMaxLenght, MinimumLength = DataConstants.RentBill.TownMinLenght)]
        public string TownOfRent { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }
    }
} 