using System.ComponentModel.DataAnnotations;

namespace GrubPix.Application.DTO
{
    // DTO for creating a new restaurant
    public class CreateRestaurantDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<MenuDto> Menus { get; set; } = new();

        [Required]
        public int OwnerId { get; set; }

    }
}