using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Filters
{
    public class QuoteWithBookCoversResultFilterAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var resultFromAction = context.Result as ObjectResult;
            if (resultFromAction?.Value == null
                || resultFromAction.StatusCode < 200
                || resultFromAction.StatusCode >= 300)
            {
                await next();
                return;
            }

            //cast to Tuple object
            var (quote, bookCovers) = ((Entities.Quote, IEnumerable<ExternalModels.BookCover>))resultFromAction.Value;

            var mappedQuote = AutoMapper.Mapper.Map<Models.QuoteWithCoversModel>(quote);
            AutoMapper.Mapper.Map(bookCovers, mappedQuote);
            resultFromAction.Value = mappedQuote;

            await next();
        }
    }
}
