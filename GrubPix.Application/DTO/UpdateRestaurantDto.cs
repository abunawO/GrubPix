using System.ComponentModel.DataAnnotations;

namespace GrubPix.Application.DTO
{
    public class UpdateRestaurantDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        //[Url(ErrorMessage = "Invalid image URL format.")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
