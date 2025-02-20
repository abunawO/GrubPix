using System.ComponentModel.DataAnnotations;

namespace GrubPix.Application.DTO
{
    public class UpdateMenuItemDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0 and less than 10,000.")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Menu ID is required.")]
        public int MenuId { get; set; }
    }
}
