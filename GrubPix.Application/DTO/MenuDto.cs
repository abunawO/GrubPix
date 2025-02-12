using System.ComponentModel.DataAnnotations;
using GrubPix.Domain.Entities;

namespace GrubPix.Application.DTO
{
    // DTO for retrieving menu details
    public class MenuDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Menu name is required.")]
        [StringLength(50, ErrorMessage = "Menu name can't exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public List<MenuItemDto> Items { get; set; } = new();
        public string Description { get; set; }
    }
}