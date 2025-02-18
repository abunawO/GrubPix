using System.ComponentModel.DataAnnotations;

namespace GrubPix.Application.DTO
{
    /// <summary>
    /// DTO for creating a new restaurant
    /// </summary>
    public class CreateRestaurantDto
    {
        [Required(ErrorMessage = "Restaurant name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        public List<MenuDto> Menus { get; set; } = new();

        [Required(ErrorMessage = "Owner ID is required.")]
        public int OwnerId { get; set; }
    }
}
