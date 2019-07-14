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

            CreateMap<SamuraiUpdateModel, Samurai>()
                //ignore SamuraiUpdateModel.Id mapping, if pass in Id is empty(default will set to 0) or other values (2,3,4)
                //Samurai.Id is primary key we don't want to chnage its original value
                .ForMember(src => src.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
