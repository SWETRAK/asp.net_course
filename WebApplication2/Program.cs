using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using WebApplication2;
using WebApplication2.Entities;
using WebApplication2.Middleware;
using WebApplication2.Models;
using WebApplication2.Models.Validators;
using WebApplication2.Services;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;

// Add services to the container.
builder.Services.AddControllers();

// Custom rules for Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasNationality", policyBuilder =>
    {
        policyBuilder.RequireClaim("Nationality");
    });
});
// Database
builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddScoped<RestaurantSeeder>();

builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// Implemented ErrorHandlingMiddleware
builder.Services.AddScoped<ErrorHandlingMiddleware>();

// AutoMapper implementation
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 

// Adding Hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Connecting FluentValidator
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add logger 
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

// Authentication service
var authenticationSettings = new AuthenticationSettings();
configuration.GetSection("Authentication").Bind(authenticationSettings);

builder.Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = "Bearer";
        option.DefaultScheme = "Bearer";
        option.DefaultChallengeScheme = "Bearer";
    }).AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = authenticationSettings.JwtIssuer,
            ValidAudience = authenticationSettings.JwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
        };
    });
builder.Services.AddSingleton<AuthenticationSettings>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>(); // Advice Controller/ Error Handler

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Running services in Program.cs 
    var serviceScopeFactory = app.Services.GetService<IServiceScopeFactory>();
    using var scope = serviceScopeFactory?.CreateScope();
    scope?.ServiceProvider.GetService<RestaurantSeeder>()?.Seed();
}

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


