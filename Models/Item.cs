using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Desc { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        // Removed Orders as we no longer need it for item creation
        // public ICollection<Order> Orders { get; set; } // Remove this line if not needed
    }
}
