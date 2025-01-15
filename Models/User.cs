using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        public string Password { get; set; }

        [MaxLength(50)]
        public string Role { get; set; }



    }
}
