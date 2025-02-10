namespace GrubPix.Application.DTO
{
    // DTO for retrieving restaurant details
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<MenuDto> Menus { get; set; } = new();
    }
}