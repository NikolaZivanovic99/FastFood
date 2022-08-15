using AutoMapper;
using DataLayer.Models;
using ServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<User, UserRegistrationDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Order, NewOrderDto>().ReverseMap();
        }
    }
}
