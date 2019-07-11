using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Profiles
{
    public class QuotesProfile : Profile
    {
        public QuotesProfile()
        {
            CreateMap<Quote, QuoteModel>()
                .ForMember(dest => dest.SamuraiName, opt => opt.MapFrom(src => src.Samurai.Name));
        }
    }
}
