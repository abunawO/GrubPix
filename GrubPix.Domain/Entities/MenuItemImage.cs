namespace GrubPix.Domain.Entities
{

    public class MenuItemImage
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

}
