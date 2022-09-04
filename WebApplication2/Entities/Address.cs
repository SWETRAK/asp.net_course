namespace WebApplication2.Entities;

public sealed class Address
{
    public int Id { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }

    public Restaurant? Restaurant { get; set; }

}