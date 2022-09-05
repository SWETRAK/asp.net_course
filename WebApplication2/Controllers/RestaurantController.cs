using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Entities;
using WebApplication2.Models;

namespace WebApplication2.Controllers;

[ApiController]
[Route("api/restaurants")]
public class RestaurantController : ControllerBase
{
    private readonly RestaurantDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public RestaurantController(RestaurantDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;

    }

    [HttpGet]
    public ActionResult<IEnumerable<RestaurantDto>> GetAll()
    {
        var restaurants = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dish)
                .ToList();

        var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);
        return Ok(restaurantsDtos);
    }

    [HttpGet("{id}")]
    public ActionResult<RestaurantDto> Get([FromRoute] int id)
    {
        var restaurant = _dbContext
            .Restaurants
            .FirstOrDefault(r => r.Id == id);

        if (restaurant is null) return NotFound();

        var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
        
        return Ok(restaurantDto);
    }

    [HttpPost]
    public ActionResult createRestaurant([FromBody] CreatedRestaurantDto dto)
    {
        var restaurant = _mapper.Map<Restaurant>(dto);

        _dbContext.Restaurants.Add(restaurant);
        _dbContext.SaveChanges();

        return Created($"/api/restaurants/{restaurant.Id}", _mapper.Map<RestaurantDto>(restaurant));
    }

}