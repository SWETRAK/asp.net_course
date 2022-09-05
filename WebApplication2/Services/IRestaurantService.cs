using WebApplication2.Models;

namespace WebApplication2.Services;

public interface IRestaurantService
{
    IEnumerable<RestaurantDto> GetAll();
    RestaurantDto GetById(int id);
    RestaurantDto Save(CreatedRestaurantDto dto);
    bool Delete(int id);
}