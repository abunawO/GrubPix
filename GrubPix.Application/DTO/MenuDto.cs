using System.ComponentModel.DataAnnotations;
using GrubPix.Domain.Entities;

namespace GrubPix.Application.DTO
{
    // DTO for retrieving menu details
    public class MenuDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Menu name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Menu name must be between 3 and 100 characters.")]
        public string Name { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public List<MenuItemDto> Items { get; set; } = new();

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description must be a maximum of 500 characters.")]
        public string Description { get; set; }
    }
}