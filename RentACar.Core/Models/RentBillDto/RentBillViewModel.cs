using System;
using System.ComponentModel.DataAnnotations;
using RentACar.Infrastructure.Data;

namespace RentACar.Core.Models.RentBillDto
{
    public class RentBillViewModel
    {
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }

        [Required]
        [StringLength(DataConstants.Car.MakeMaxLenght, MinimumLength = DataConstants.Car.MakeMinLenght)]
        public string CarMake { get; set; } = null!;

        [Required]
        [StringLength(DataConstants.Car.ModelMaxLenght, MinimumLength = DataConstants.Car.ModelMinLenght)]
        public string CarModel { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string UserFirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string UserLastName { get; set; } = null!;

        [Required]
        [StringLength(DataConstants.RentBill.TownMaxLenght, MinimumLength = DataConstants.RentBill.TownMinLenght)]
        public string TownOfRent { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime DateOfTaking { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfReturn { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StartMileage { get; set; }

        [Range(0, int.MaxValue)]
        public int? EndMileage { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        public int KilometersDriven { get; set; } // This is a calculated property from the entity
    }
} 