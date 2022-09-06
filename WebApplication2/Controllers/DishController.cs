using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers;

[ApiController]
[Route("api/restaurants/{Id}/dish")]
public class DishController : ControllerBase
{

    private readonly IDishService _dishService;

    public DishController(IDishService dishService)
    {
        _dishService = dishService;
    }

    [HttpPost]
    public ActionResult Post([FromRoute(Name = "Id")] int restaurantId, [FromBody] CreateDishDto dto)
    {
        var dishId = _dishService.Create(restaurantId, dto);
        return Created($"api/restaurants/{restaurantId}/dish/{dishId}", null);
    }

    [HttpGet]
    public ActionResult<List<DishDto>> Get([FromRoute(Name = "Id")] int restaurantId)
    {
        var dishes = _dishService.GetAll(restaurantId);
        return Ok(dishes);
    }

    [HttpGet("{dishId}")]
    public ActionResult<DishDto> Get([FromRoute(Name = "Id")] int restaurantId, [FromRoute] int dishId)
    {
        var dish = _dishService.GetById(restaurantId, dishId);
        return Ok(dish);
    }

    [HttpDelete]
    public ActionResult Delete([FromRoute(Name = "Id")] int restaurantId)
    {
        _dishService.RemoveAll(restaurantId);
        return NoContent();
    }
}