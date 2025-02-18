using System.ComponentModel.DataAnnotations;

namespace GrubPix.Application.DTO
{
    /// <summary>
    /// DTO for creating a menu
    /// </summary>
    public class CreateMenuDto
    {
        [Required(ErrorMessage = "Menu name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "RestaurantId is required.")]
        public int RestaurantId { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;
    }
}
