using System.ComponentModel.DataAnnotations;

namespace GrubPix.Application.DTO
{
    public class FavoriteMenuItemDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Menu item ID is required.")]
        public int MenuItemId { get; set; }

        [Required(ErrorMessage = "Menu item name is required.")]
        [StringLength(100, ErrorMessage = "Menu item name cannot exceed 100 characters.")]
        public string MenuItemName { get; set; } = string.Empty;
    }
}
