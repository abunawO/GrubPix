using AutoMapper;
using GrubPix.Domain.Entities;   // Your domain models
using GrubPix.Application.DTO;
using GrubPix.Application.DTOs;
using GrubPix.Application.Features.User;   // DTOs you'll map to

namespace GrubPix.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping for Menus
            CreateMap<CreateMenuDto, Menu>();
            CreateMap<Menu, MenuDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.MenuItems));
            CreateMap<UpdateMenuDto, Menu>();

            // Mapping for Restaurants
            CreateMap<CreateRestaurantDto, Restaurant>();
            CreateMap<Restaurant, RestaurantDto>();
            CreateMap<UpdateRestaurantDto, Restaurant>();

            // Mapping for Menu Items
            CreateMap<CreateMenuItemDto, MenuItem>();
            CreateMap<MenuItem, MenuItemDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));
            CreateMap<MenuItemImage, MenuItemImageDto>();
            CreateMap<UpdateMenuItemDto, MenuItem>();

            // Mapping for Users
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();

            // Mapping for Customers
            CreateMap<Customer, CustomerDto>();
            CreateMap<UpdateCustomerDto, Customer>();

            // Map Customer -> CustomerDto
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, UserDto>();

            // Mapping for Favorites
            CreateMap<FavoriteMenuItem, FavoriteMenuItemDto>()
                .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem.Name));

            CreateMap<AddFavoriteRequest, FavoriteMenuItem>();

            // Mapping for Authentication
            CreateMap<RegisterCommand, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));

            CreateMap<LoginCommand, UserDto>();
        }
    }
}
