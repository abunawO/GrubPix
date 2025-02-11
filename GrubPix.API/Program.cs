using GrubPix.API.Configuration;
using Microsoft.EntityFrameworkCore;
using GrubPix.Infrastructure.Persistence;
using Amazon.S3;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injection
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddCoreApplicationServices();
builder.Services.AddCoreInfrastructureServices();

// Database Configuration with Enhanced Logging
builder.Services.AddDbContext<GrubPixDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("GrubPix.Infrastructure"))
           .LogTo(Console.WriteLine, LogLevel.Information) // Enable SQL query logging
           .EnableSensitiveDataLogging()                  // Logs parameter values for better debugging
           .EnableDetailedErrors();                       // Provides detailed error information
});

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// HTTP Logging
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

var app = builder.Build();

// HTTP Request Pipeline
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
app.UseRouting();
app.UseAuthorization();
app.UseHttpLogging();

app.MapControllers();

app.Run();
