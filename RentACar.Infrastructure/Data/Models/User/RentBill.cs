using RentACar.Infrastructure.Data.Models.Vehicle;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentACar.Infrastructure.Data.Models.User
{
    public class RentBill
    {
        /// <summary>
        /// Rent bill identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Town of rent
        /// </summary>
        [Required]
        [MinLength(DataConstants.RentBill.TownMinLenght)]
        [MaxLength(DataConstants.RentBill.TownMaxLenght)]
        public string TownOfRent { get; set; } = null!;

        /// <summary>
        /// Date of taking rent
        /// </summary>
        [Required]
        public DateTime DateOfTaking { get; set; }

        /// <summary>
        /// Date of returning rent
        /// </summary>
        public DateTime? DateOfReturn { get; set; }

        /// <summary>
        /// User identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// User of rent
        /// </summary>
        [Required]
        public ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Car identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(Car))]
        public int CarId { get; set; }

        /// <summary>
        /// Car of rent
        /// </summary>
        [Required]
        public Car Car { get; set; } = null!;

        /// <summary>
        /// Starting mileage when car is rented
        /// </summary>
        [Required]
        public int StartMileage { get; set; }

        /// <summary>
        /// Ending mileage when car is returned
        /// </summary>
        public int? EndMileage { get; set; }

        /// <summary>
        /// Total price for the rental
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Total kilometers driven during rental
        /// </summary>
        [NotMapped]
        public int KilometersDriven => EndMileage.HasValue ? EndMileage.Value - StartMileage : 0;
    }
}
