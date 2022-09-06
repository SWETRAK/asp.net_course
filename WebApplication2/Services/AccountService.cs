﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication2.Entities;
using WebApplication2.Models;

namespace WebApplication2.Services;

public interface IAccountService
{
    void RegisterUser(RegisterUserDto dto);
    string GenerateJwt(LoginDto dto);
}

public class AccountService : IAccountService
{
    private readonly RestaurantDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthenticationSettings _authenticationSettings;

    public AccountService(
        RestaurantDbContext dbContext, 
        IMapper mapper, 
        IPasswordHasher<User> passwordHasher,
        AuthenticationSettings authenticationSettings
        )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
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

    public string GenerateJwt(LoginDto dto)
    {
        var user = _dbContext
            .Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Email == dto.Email);

        if (user is null) throw new Exception("Invalid user name or password");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed) throw new Exception("Invalid user name or password");

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")),
            
        };

        if (!string.IsNullOrEmpty(user.Nationality))
        {
            claims.Add(new Claim("Nationality", user.Nationality));
        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

        var token = new JwtSecurityToken(
            _authenticationSettings.JwtIssuer,
            _authenticationSettings.JwtIssuer,
            claims,
            expires: expires,
            signingCredentials: cred
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

}