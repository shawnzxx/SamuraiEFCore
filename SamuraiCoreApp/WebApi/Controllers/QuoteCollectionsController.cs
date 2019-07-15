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
using Microsoft.AspNetCore.Http;

namespace WebApi.Controllers
{
    [Produces("application/json", "application/xml")]
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

        /// <summary>
        /// Add list of quotes to the sepecific samurai
        /// </summary>
        /// <param name="quoteCollection">quoteCollection model</param>
        /// <returns></returns>
        [HttpPost]
        [QuotesResultFilter]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<IEnumerable<Quote>>> CreateQuoteCollection([FromBody] IEnumerable<QuoteCreationModel> quoteCollection) {
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
        /// <summary>
        /// Get quotes collections
        /// </summary>
        /// <param name="quoteIds">Passing in quote ids as (id1, id2, id3, ...)</param>
        /// <returns>Return list of quites</returns>
        [HttpGet]
        [Route("({quoteIds})", Name = "GetQuoteCollections")]
        [QuotesResultFilter]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<Quote>>> GetQuoteCollections([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<int> quoteIds)
        {
            var quotesEntities = await _quoteRepository.GetQuotesAsync(quoteIds);

            if (quoteIds.Count() != quotesEntities.Count()) {
                return NoContent();
            }

            return Ok(quotesEntities);
        }
    }
}