using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WebApplication2.Entities;
using WebApplication2.Models;

namespace WebApplication2.Services;

public interface IAccountService
{
    void RegisterUser(RegisterUserDto dto);
}

public class AccountService : IAccountService
{
    private readonly RestaurantDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountService(RestaurantDbContext dbContext, IMapper mapper, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public void RegisterUser(RegisterUserDto dto)
    {
        
        var newUser = new User()
        {
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            Nationality = dto.Nationality,
            RoleId = dto.RoleId,
        };

        var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
        newUser.PasswordHash = hashedPassword;
        
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();
    }


}