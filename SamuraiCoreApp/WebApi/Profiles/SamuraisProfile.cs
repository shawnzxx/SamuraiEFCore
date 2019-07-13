using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Profiles
{
    public class SamuraisProfile : Profile
    {
        public SamuraisProfile()
        {
            CreateMap<Samurai, SamuraiModel>()
                .ForMember(dest => dest.QuoteCounts, opt => opt.MapFrom(src => src.Quotes.Count));



            CreateMap<SamuraiCreationModel, Samurai>();
        }
    }
}
