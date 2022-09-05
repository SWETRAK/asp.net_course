using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Entities;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers;

[ApiController]
[Route("api/restaurants")]
public class RestaurantController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;
    
    public RestaurantController(
        IRestaurantService restaurantService
    ) {
        _restaurantService = restaurantService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<RestaurantDto>> GetAll()
    {
        var restaurantsDto = _restaurantService.GetAll();
        return Ok(restaurantsDto);
    }

    [HttpGet("{id}")]
    public ActionResult<RestaurantDto> Get([FromRoute] int id)
    {
        var restaurantDto = _restaurantService.GetById(id);
        if (restaurantDto is null) return NotFound();
        return Ok(restaurantDto);
    }

    [HttpPost]
    public ActionResult CreateRestaurant([FromBody] CreatedRestaurantDto dto)
    {
        // Walidacja api
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var restaurant = _restaurantService.Save(dto);
        return Created($"/api/restaurants/{restaurant.Id}", restaurant);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteRestaurant([FromRoute] int id)
    {
        var isDeleted = _restaurantService.Delete(id);

        if (isDeleted)
        {
            return NoContent();
        }

        return NotFound();
    }

}