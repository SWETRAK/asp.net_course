using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Controllers;
using WebApplication2.Entities;
using WebApplication2.Models;

namespace WebApplication2.Services;

public class RestaurantService : IRestaurantService
{
    private readonly RestaurantDbContext _dbContext;
    private readonly IMapper _mapper;

    public RestaurantService(RestaurantDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public IEnumerable<RestaurantDto> GetAll()
    {
        var restaurants = _dbContext
            .Restaurants
            .Include(r => r.Address)
            .Include(r => r.Dish)
            .ToList();

        return _mapper.Map<List<RestaurantDto>>(restaurants);
    }

    public RestaurantDto GetById(int id)
    {
        var restaurant = _dbContext
            .Restaurants
            .FirstOrDefault(r => r.Id == id);

        return restaurant is null ? null : _mapper.Map<RestaurantDto>(restaurant);
    }

    public RestaurantDto Save(CreatedRestaurantDto dto)
    {
        var restaurant = _mapper.Map<Restaurant>(dto);

        _dbContext.Restaurants.Add(restaurant);
        _dbContext.SaveChanges();
        return _mapper.Map<RestaurantDto>(restaurant);
    }

    public bool Delete(int id)
    {
        var restaurant = _dbContext
            .Restaurants
            .FirstOrDefault(r => r.Id == id);

        if (restaurant is null) return false;

        _dbContext.Restaurants.Remove(restaurant);
        _dbContext.SaveChanges();
        return true;
    }

}