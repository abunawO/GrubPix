using System.ComponentModel.DataAnnotations;

namespace GrubPix.Application.DTO
{
    // DTO for retrieving restaurant details
    public class RestaurantDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Restaurant name is required.")]
        [StringLength(100, ErrorMessage = "Name can't exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address can't exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public List<MenuDto> Menus { get; set; } = new();

        [Url(ErrorMessage = "Invalid URL format.")]
        public string ImageUrl { get; set; } = string.Empty; // Added
    }
}