using System.Collections.Generic;

namespace GrubPix.Domain.Entities
{
    public class Customer : BaseUser
    {
        public ICollection<FavoriteMenuItem> FavoriteMenuItems { get; set; } = new List<FavoriteMenuItem>();
    }
}
