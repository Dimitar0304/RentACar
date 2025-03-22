using System.ComponentModel.DataAnnotations;

namespace RentACar.Infrastructure.Data.Models.Vehicle
{
    public class Category
    {
        /// <summary>
        /// Car category id
        /// </summary>
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
        public IEnumerable<Car> Cars { get; set; } = null!;
    }
}
