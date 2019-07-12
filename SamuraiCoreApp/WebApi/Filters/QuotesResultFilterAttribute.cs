using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Filters
{
    public class QuotesResultFilterAttribute : ResultFilterAttribute
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

            resultFromAction.Value = AutoMapper.Mapper.Map<IEnumerable<QuoteOutPutModel>>(resultFromAction.Value);

            await next();
        }
    }
}
