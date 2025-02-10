using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace GrubPix.API.Configuration
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GrubPix API",
                    Version = "v1",
                    Description = "API documentation for GrubPix - A Visual Menu Platform",
                    Contact = new OpenApiContact
                    {
                        Name = "GrubPix Team",
                        Email = "support@grubpix.com",
                        Url = new Uri("https://grubpix.com")
                    }
                });

                // Optional: Add support for JWT Authentication if needed
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }
    }
}
