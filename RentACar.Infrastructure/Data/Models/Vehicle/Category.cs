using System.ComponentModel.DataAnnotations;

namespace RentACar.Infrastructure.Data.Models.Vehicle
{
    public class Category
    {
        /// <summary>
        /// Car category id
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Car category name
        /// </summary>
        [Required]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Navigation property
        /// </summary>
        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
