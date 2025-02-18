using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrubPix.Domain.Entities
{
    public class User : BaseUser
    {
        public List<Restaurant> Restaurants { get; set; } = new();
    }
}
