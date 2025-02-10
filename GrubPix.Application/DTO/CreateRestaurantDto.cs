namespace GrubPix.Application.DTO
{
    // DTO for creating a new restaurant
    public class CreateRestaurantDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}