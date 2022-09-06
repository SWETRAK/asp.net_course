using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Entities;
using WebApplication2.Models;

namespace WebApplication2.Services;

public interface IDishService
{
    int Create(int restaurantId, CreateDishDto dto);

    DishDto GetById(int restaurantId, int dishId);

    List<DishDto> GetAll(int restaurantId);

    void RemoveAll(int restaurantId);
}

public class DishService : IDishService
{

    private readonly RestaurantDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public DishService(RestaurantDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public int Create(int restaurantId, CreateDishDto dto)
    {
        var restaurant = GetRestaurantById(restaurantId);

        var dishEntity = _mapper.Map<Dish>(dto);

        _dbContext.Dishes.Add(dishEntity);
        _dbContext.SaveChanges();

        return dishEntity.Id;
    }

    public DishDto GetById(int restaurantId, int dishId)
    {
        var restaurant = GetRestaurantById(restaurantId);

        var dish = _dbContext.Dishes.FirstOrDefault(d => d.Id == dishId);
        if (dish is null || dish.RestaurantId != restaurantId) throw new Exception("Dish not found");

        var dishDto = _mapper.Map<DishDto>(dish);
        return dishDto;
    }

    public List<DishDto> GetAll(int restaurantId)
    {
        var restaurant = GetRestaurantById(restaurantId);

        var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dish);
        return dishDtos;
    }

    public void RemoveAll(int restaurantId)
    {
        var restaurant = GetRestaurantById(restaurantId);
        
        _dbContext.RemoveRange(restaurant.Dish);
        _dbContext.SaveChanges();
    }

    private Restaurant GetRestaurantById(int restaurantId)
    {
        var restaurant = _dbContext
            .Restaurants
            .Include(r => r.Dish)
            .FirstOrDefault(r => r.Id == restaurantId);
        if (restaurant is null) throw new Exception("Restaurant not found");
        return restaurant;
    }
}