namespace GrubPix.Domain.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int MenuId { get; set; }
        public Menu Menu { get; set; }  // Navigation property
    }

}
