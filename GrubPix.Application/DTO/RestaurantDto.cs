using System.ComponentModel.DataAnnotations;

namespace GrubPix.Application.DTO
{
    // DTO for retrieving restaurant details
    public class RestaurantDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Restaurant name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address must be a maximum of 255 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description must be a maximum of 500 characters.")]
        public string Description { get; set; } = string.Empty;
        public List<MenuDto> Menus { get; set; } = new();

        [Url(ErrorMessage = "Invalid URL format.")]
        public string ImageUrl { get; set; } = string.Empty; // Added
    }
}