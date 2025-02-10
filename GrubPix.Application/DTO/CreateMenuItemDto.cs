namespace GrubPix.Application.DTO
{
    // DTO for creating a menu item
    public class CreateMenuItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MenuId { get; set; }
    }
}