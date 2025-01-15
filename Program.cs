using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Product.Data;
using Product.Models.Interfaces;
using Product.Models;
using Serilog;
using System.Text;
using ProductsApi;
using ProductsApi.Models.Interfaces;
using ProductsApi.Data;
using ProductsApi.Models.Interfaces;
using ProductsApi.Repository;

var builder = WebApplication.CreateBuilder(args);

// Set up Serilog for logging to a JSON file
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/request_logs.json", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
    .WriteTo.File("logs/response_logs.json", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Registering services for Dependency Injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));  // Registering Generic Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();  // User Repository
builder.Services.AddScoped<IItemRepository, ItemRepository>();  // Item Repository
builder.Services.AddScoped<IOrderRepository, OrderRepository>(); // Add OrderRepository
builder.Services.AddScoped<IEmailRepository, EmailRepository>();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Correctly set the OpenAPI version to 3.0.0
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product API",
        Version = "1.0.0",  // OpenAPI Version
        Description = "API for managing products"
    });

    // Add security definition for Bearer authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter your Bearer token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    // Add security requirement for Bearer authentication
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

// Add Authorization Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Add CORS for cross-origin requests
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://localhost:7295") // Change this to your front-end URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Build the application
var app = builder.Build();

// Use Swagger for API documentation in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
    });
}

// Use CORS policy
app.UseCors("AllowSpecificOrigins");

// Middleware pipeline: Logging and Authorization
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Custom middlewares for logging requests and responses
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ResponseLoggingMiddleware>();

// Map controllers (API endpoints)
app.MapControllers();

// Run the application
app.Run();

// --- Custom Middlewares for Request and Response Logging ---

// Request Logging Middleware
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
        await _next(context);
    }
}

// Response Logging Middleware
public class ResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseLoggingMiddleware> _logger;

    public ResponseLoggingMiddleware(RequestDelegate next, ILogger<ResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        using (var newBodyStream = new MemoryStream())
        {
            context.Response.Body = newBodyStream;
            await _next(context);
            newBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(newBodyStream).ReadToEnd();
            _logger.LogInformation("Response: {StatusCode} {ResponseBody}", context.Response.StatusCode, responseBody);
            newBodyStream.Seek(0, SeekOrigin.Begin);
            await newBodyStream.CopyToAsync(originalBodyStream);
        }
    }
}
