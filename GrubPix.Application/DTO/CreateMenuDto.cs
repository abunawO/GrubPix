namespace GrubPix.Application.DTO
{
    // DTO for creating a menu
    public class CreateMenuDto
    {
        public string Name { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public object Description { get; internal set; }
    }
}