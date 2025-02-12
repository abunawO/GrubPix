using System.ComponentModel.DataAnnotations;
using GrubPix.Domain.Entities;

namespace GrubPix.Application.DTO
{
    // DTO for retrieving menu item details
    public class MenuItemDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(100, ErrorMessage = "Item name can't exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Description can't exceed 300 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 1000, ErrorMessage = "Price must be between $0.01 and $1000.")]
        public decimal Price { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        public int MenuId { get; set; }
        public string ImageUrl { get; set; } = string.Empty; // Added
    }
}