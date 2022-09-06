using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using NLog.Web;
using WebApplication2.Entities;
using WebApplication2.Middleware;
using WebApplication2.Models;
using WebApplication2.Models.Validators;
using WebApplication2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


