namespace GrubPix.Application.DTO
{
    // DTO for creating a menu
    public class CreateMenuDto
    {
        public string Name { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}