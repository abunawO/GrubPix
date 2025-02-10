using Microsoft.Extensions.DependencyInjection;
using GrubPix.Application.Services;
using GrubPix.Infrastructure.Persistence;
using GrubPix.Infrastructure.Repositories;
using GrubPix.Infrastructure.Services;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Application.Mappings;

namespace GrubPix.API.Configuration
{
    public static class DependencyInjection
    {
        // Application Layer Services
        public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IRestaurantService, RestaurantService>();

            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }

        // Infrastructure Layer Services
        public static IServiceCollection AddCoreInfrastructureServices(this IServiceCollection services)
        {
            // Register Infrastructure Services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IImageStorageService, ImageStorageService>();

            // Register Repositories
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IRestaurantRepository, RestaurantRepository>();

            // Register DbContext
            services.AddScoped<GrubPixDbContext>();

            return services;
        }
    }
}
