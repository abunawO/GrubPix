using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Serilog;
using GrubPix.Infrastructure;
using CorrelationId.DependencyInjection;
using Amazon.S3;
using GrubPix.API.Configuration;
using GrubPix.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using GrubPix.API.Middleware;
using CorrelationId; // Adjust if needed

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .WriteTo.Console()
    .WriteTo.File("logs/grubpix-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Register Correlation ID service
builder.Services.AddDefaultCorrelationId();

// Middleware to Generate Correlation ID
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Dependency Injection
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddCoreApplicationServices();
builder.Services.AddCoreInfrastructureServices();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Database Configuration with Enhanced Logging
builder.Services.AddDbContext<GrubPixDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("GrubPix.Infrastructure"))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();
});

// Form Options Configuration
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocumentation(); // Ensure this method is defined in your project

// Response Caching
builder.Services.AddResponseCaching();

// HTTP Logging
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

// -------------------- REPLACED JWT CONFIGURATION WITH CLERK AUTH --------------------

// // Load Clerk settings from appsettings.json
// var clerkSettings = builder.Configuration.GetSection("Clerk");
// var clerkIssuer = clerkSettings["Issuer"];
// var clerkAudience = clerkSettings["Audience"];

// // Configure Clerk Authentication
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Authority = clerkIssuer;
//         options.Audience = clerkAudience;
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidIssuer = clerkIssuer,
//             ValidateAudience = true,
//             ValidAudience = clerkAudience,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true
//         };
//     });

builder.Services.AddAuthorization();

// -------------------- COMMENTED OUT OLD JWT AUTHENTICATION --------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// -------------------- END OF OLD JWT CONFIGURATION --------------------

var app = builder.Build();

// Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Ensure middleware is used correctly
app.UseCorrelationId();

// Use Serilog Request Logging
app.UseSerilogRequestLogging();

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

// Use Response Caching and Security Middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseResponseCaching();
app.UseCors("AllowAll");

// Enable Clerk Authentication
app.UseAuthentication();
app.UseAuthorization();

// Logging Incoming Requests
app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
    await next();
});

// Map Controllers
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
