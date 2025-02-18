using System.ComponentModel.DataAnnotations;

namespace GrubPix.Application.DTO
{
    /// <summary>
    /// DTO for creating a menu item
    /// </summary>
    public class CreateMenuItemDto
    {
        [Required(ErrorMessage = "Menu item name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "MenuId is required.")]
        public int MenuId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
    }
}
