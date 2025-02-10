namespace GrubPix.Domain.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Menu> Menus { get; set; } = new List<Menu>();
        public string ImageUrl { get; set; } = string.Empty;// New property for menu item image
    }
}
