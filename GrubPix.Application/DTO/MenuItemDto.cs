namespace GrubPix.Application.DTO
{
    // DTO for retrieving menu item details
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MenuId { get; set; }
    }
}