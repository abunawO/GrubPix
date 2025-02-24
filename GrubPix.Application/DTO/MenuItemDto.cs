using System.ComponentModel.DataAnnotations;
using GrubPix.Domain.Entities;

namespace GrubPix.Application.DTO
{
    // DTO for retrieving menu item details
    public class MenuItemDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Item name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description must be a maximum of 500 characters.")]
        public string Description { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Menu ID is required.")]
        public int MenuId { get; set; }

        public ICollection<MenuItemImageDto> Images { get; set; } = new List<MenuItemImageDto>();
    }
}