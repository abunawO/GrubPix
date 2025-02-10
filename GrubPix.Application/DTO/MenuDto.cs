namespace GrubPix.Application.DTO
{
    // DTO for retrieving menu details
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public List<MenuItemDto> Items { get; set; } = new();
        public string Description { get; set; }
    }
}