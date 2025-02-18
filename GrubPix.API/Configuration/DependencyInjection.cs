using GrubPix.Application.Services;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Infrastructure.Repositories;
using GrubPix.Infrastructure.Services;
using GrubPix.Application.Mappings;
using GrubPix.Infrastructure.Persistence;
using MediatR;
using GrubPix.Application.Features.Restaurant;
using GrubPix.Application.Features.MenuItem;
using Amazon.S3;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Application.Interfaces;

namespace GrubPix.API.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
        {
            // AWS S3 Client Registration
            services.AddAWSService<IAmazonS3>();

            // Application Services
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IImageStorageService, S3Service>();
            services.AddScoped<IMenuItemService, MenuItemService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();

            // AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddAutoMapper(typeof(MappingProfile));

            // MediatR Handlers
            services.AddMediatR(typeof(GetRestaurantsQueryHandler).Assembly);
            services.AddMediatR(typeof(GetAllMenuItemsQuery).Assembly);

            return services;
        }

        public static IServiceCollection AddCoreInfrastructureServices(this IServiceCollection services)
        {
            // Infrastructure Services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IImageStorageService, S3Service>();

            // Repositories
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IMenuItemRepository, MenuItemRepository>();
            services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();


            // Database Context
            services.AddScoped<GrubPixDbContext>();

            return services;
        }
    }
}
