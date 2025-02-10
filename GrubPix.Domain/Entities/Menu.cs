
namespace GrubPix.Domain.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
        // Navigation property
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public string Description { get; set; }

    }
}
