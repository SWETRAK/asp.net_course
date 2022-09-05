using AutoMapper;
using WebApplication2.Entities;
using WebApplication2.Models;

namespace WebApplication2.Mappers;

public class RestaurantMappingProfile: Profile
{
    public RestaurantMappingProfile()
    {
        CreateMap<Restaurant, RestaurantDto>()
            .ForMember(m => m.City, c => c.MapFrom(s => s.Address.City))
            .ForMember(m => m.Street, c => c.MapFrom(s => s.Address.Street))
            .ForMember(m => m.PostalCode, c => c.MapFrom(s => s.Address.PostalCode));

        CreateMap<CreatedRestaurantDto, Restaurant>()
            .ForMember(m => m.Address, c => c.MapFrom(dto => new Address()
            {
                City = dto.City,
                PostalCode = dto.PostalCode,
                Street = dto.Street
            }));

        CreateMap<Dish, DishDto>();
    }
}