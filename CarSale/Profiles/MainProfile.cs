using AutoMapper;
using CarSale.Models;
using CarSale.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Profiles
{
    public class MainProfile : Profile
    {
        public MainProfile()
        {
            CreateMap<RegisterUserDTO,ApplicationUser>().ForMember(dest=>dest.DateOfCreation,
                                                                    opts=>opts.MapFrom(src=> DateTime.UtcNow));

            CreateMap<ApplicationUser, ReadUserDTO>().ReverseMap();

            CreateMap<CarModel, CarReadDTO>();

            CreateMap<CarUpsertDTO, CarModel>().ForMember(dest=>dest.Photos,
                opts=>opts.MapFrom((src,member)=> {
                    return src.Photos.Split(',').ToList();
                }
                ))
                .ReverseMap();
        }
    }
}
