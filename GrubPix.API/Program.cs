using GrubPix.API.Configuration;
using Microsoft.EntityFrameworkCore;
using GrubPix.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Infrastructure.Repositories;
using GrubPix.Application.Features.Restaurant;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Database Configuration with Enhanced Logging
builder.Services.AddDbContext<GrubPixDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("GrubPix.Infrastructure"));

    // Enable SQL query logging
    options.LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging()  // Logs parameter values for better debugging
           .EnableDetailedErrors();       // Provides detailed error information
});


// Dependency Injection
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMediatR(typeof(GetRestaurantsQueryHandler).Assembly);
builder.Services.AddCoreApplicationServices();
builder.Services.AddCoreInfrastructureServices();

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// Logging
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

var app = builder.Build();

// HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GrubPix API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseRouting();
app.UseHttpLogging();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();