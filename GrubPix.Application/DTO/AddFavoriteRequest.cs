using System.ComponentModel.DataAnnotations;

namespace GrubPix.Application.DTO
{
    public class AddFavoriteRequest
    {
        [Required]
        public int MenuItemId { get; set; }
    }
}
