using AutoMapper;
using System.Collections.Generic;

namespace WebApi.Profiles
{
    public class QuotesProfile : Profile
    {
        public QuotesProfile()
        {
            CreateMap<Entities.Quote, Models.QuoteModel>()
                .ForMember(dest => dest.SamuraiName, opt => opt.MapFrom(src => src.Samurai.Name));

            CreateMap<Models.QuoteCreationModel, Entities.Quote>();

            //QuoteWithCoversModel have two classes, one set data from QuoteModle, one set of data from BookCoversModel
            //we need two mapper below
            CreateMap<Entities.Quote, Models.QuoteWithCoversModel>()
                .ForMember(dest => dest.SamuraiName, opt => opt.MapFrom(src => src.Samurai.Name));

            CreateMap<IEnumerable<ExternalModels.BookCover>, Models.QuoteWithCoversModel>()
                .ForMember(dest => dest.QuoteCovers, opt => opt.MapFrom(src => src));
        }
    }
}
