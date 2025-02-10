using AutoMapper;
using GrubPix.Domain.Entities;       // Your domain models
using GrubPix.Application.DTO;      // DTOs you'll map to

namespace GrubPix.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Example Mappings
            CreateMap<Menu, MenuDto>().ReverseMap();
            CreateMap<Restaurant, RestaurantDto>().ReverseMap();
        }
    }
}
