namespace WebApplication2.Entities;

public sealed class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public bool HasDelivery { get; set; }
    public string ContactEmail { get; set; }
    public string ContactNumber { get; set; }

    public int AddressId { get; set; }
    
    public Address Address { get; set; }

    public List<Dish> Dish { get; set; }
}