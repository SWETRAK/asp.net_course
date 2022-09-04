namespace WebApplication2.Entities;

public class RestaurantSeeder
{
    private readonly RestaurantDbContext _restaurantDbContext;

    public RestaurantSeeder(RestaurantDbContext restaurantDbContext)
    {
        _restaurantDbContext = restaurantDbContext;
    }
    
    public void Seed()
    {
        if (_restaurantDbContext.Restaurants != null && _restaurantDbContext.Database.CanConnect() && !_restaurantDbContext.Restaurants.Any())
        {
            var restaurants = GetRestaurants();
            _restaurantDbContext.Restaurants.AddRange(restaurants);
            _restaurantDbContext.SaveChanges();
        }
    }

    private IEnumerable<Restaurant> GetRestaurants()
    {
        var restaurants = new List<Restaurant>()
        {
            new Restaurant()
            {
                Name = "KFC",
                Category = "Fast Food",
                Description = "KFC",
                ContactEmail = "kfc@email.pl",
                HasDelivery = true,
                Dish = new List<Dish>
                {
                    new Dish()
                    {
                        Name = "Nashville Hot Chicken",
                        Price = 10.30M,
                    },
                    new Dish()
                    {
                        Name = "Chicken Nuggets",
                        Price = 5.30M,
                    }
                },
                Address = new Address()
                {
                    City = "Lublin",
                    Street = "Długa 5",
                    PostalCode = "20-234"
                }
            }
        };
        return restaurants;
    }
}