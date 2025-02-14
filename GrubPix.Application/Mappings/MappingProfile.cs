using AutoMapper;
using GrubPix.Domain.Entities;   // Your domain models
using GrubPix.Application.DTO;   // DTOs you'll map to

namespace GrubPix.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping for Menus
            CreateMap<CreateMenuDto, Menu>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<Menu, CreateMenuDto>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<Menu, MenuDto>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.MenuItems)); // Ensure MenuItems are mapped

            CreateMap<MenuDto, Menu>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.MenuItems, opt => opt.MapFrom(src => src.Items)); // Reverse mapping

            // Mapping for Restaurants with ImageUrl
            CreateMap<CreateRestaurantDto, Restaurant>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<Restaurant, CreateRestaurantDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Menus, opt => opt.MapFrom(src => src.Menus)); // Ensure Menus are mapped

            CreateMap<RestaurantDto, Restaurant>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Menus, opt => opt.MapFrom(src => src.Menus)); // Reverse mapping

            // Mapping for MenuItems with ImageUrl
            CreateMap<MenuItemDto, MenuItem>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<MenuItem, MenuItemDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<CreateMenuItemDto, MenuItem>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<MenuItem, CreateMenuItemDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

            // Mapping for User
            CreateMap<User, UserDto>();
        }
    }
}
