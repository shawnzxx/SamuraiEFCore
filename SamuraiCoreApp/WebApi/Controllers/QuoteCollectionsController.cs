using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using WebApi.Filters;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteCollectionsController : ControllerBase
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;

        public QuoteCollectionsController(IQuoteRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        [QuotesResultFilter]
        public async Task<IActionResult> CreateQuoteCollection([FromBody] IEnumerable<QuoteCreationModel> quoteCollection) {
            var quoteEntities = _mapper.Map<IEnumerable<Quote>>(quoteCollection);

            foreach (var quoteEntity in quoteEntities)
            {
                _quoteRepository.AddQuote(quoteEntity);
            }

            await _quoteRepository.SaveChangeAsync();

            var quotesToReturn = await _quoteRepository.GetQuotesAsync(
                quoteEntities.Select(q => q.Id).ToList());

            var quoteIds = string.Join(",", quotesToReturn.Select(q => q.Id));

            return CreatedAtRoute("GetQuoteCollections",
                new { quoteIds = quoteIds },
                quotesToReturn);
        }

        //api/quotecollections/(id1, id2, id3, ...)
        [HttpGet]
        [Route("({quoteIds})", Name = "GetQuoteCollections")]
        [QuotesResultFilter]
        public async Task<IActionResult> GetQuoteCollections([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<int> quoteIds)
        {
            var quotesEntities = await _quoteRepository.GetQuotesAsync(quoteIds);

            if (quoteIds.Count() != quotesEntities.Count()) {
                return NoContent();
            }

            return Ok(quotesEntities);
        }
    }
}