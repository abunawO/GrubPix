using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrubPix.Domain.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }  // Hashed password, never store plain text passwords

        [Required]
        public string Role { get; set; } = "RestaurantOwner";  // Default role is "User"

        // New relationship: A user can own multiple restaurants
        public List<Restaurant> Restaurants { get; set; } = new();
    }
}
