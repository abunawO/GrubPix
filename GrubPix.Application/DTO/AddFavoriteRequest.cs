using System.ComponentModel.DataAnnotations;

namespace GrubPix.Application.DTOs
{
    public class AddFavoriteRequest
    {
        [Required]
        public int MenuItemId { get; set; }
    }
}
