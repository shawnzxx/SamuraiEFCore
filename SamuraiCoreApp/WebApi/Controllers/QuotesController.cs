using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApi.Contexts;
using WebApi.Entities;
using WebApi.ExternalModels;
using WebApi.Filters;
using WebApi.Models;
using WebApi.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Produces("application/json", "application/xml")]
    [Route("api/quotes")]
    [ApiController]
    public class QuotesController : ControllerBase
    {

        private SamuraiContext _context;
        private ILogger<SamuraisController> _logger;
        private ISamuraiRepository _samuraiRepository;
        private IQuoteRepository _quoteRepository;
        private IMapper _mapper;

        public QuotesController(SamuraiContext context,
            ILogger<SamuraisController> logger,
            IQuoteRepository quoteRepository,
            ISamuraiRepository samuraiRepository,
            IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _samuraiRepository = samuraiRepository ?? throw new ArgumentNullException(nameof(samuraiRepository));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Get full list of quotes
        /// </summary>
        /// <returns>Full list of quites in database</returns>
        // GET: api/quotes
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        //[QuotesResultFilter]
        public async Task<ActionResult<IEnumerable<QuoteModel>>> GetQuotes()
        {
            try
            {
                var quoteEntities = await _quoteRepository.GetQuotesAsync();
                return Ok(_mapper.Map<IEnumerable<QuoteModel>>(quoteEntities));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get quote by quiteId
        /// </summary>
        /// <param name="quoteId">Id of quote</param>
        /// <returns>return requested quote</returns>
        // GET: api/quotes/2
        [HttpGet]
        [QuoteWithBookCoversResultFilter]
        [Route("{quoteId}", Name = "GetQuote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuoteWithCoversModel>> GetQuote(int quoteId)
        {
            try
            {
                var quoteEntity = await _quoteRepository.GetQuoteAsync(quoteId);
                if (quoteEntity == null)
                {
                    return NotFound();
                }

                //test with get single bookcover request
                //var bookCover = await _quoteRepository.GetBookCoverTestAsync(quoteEntity.Text);

                //get set of bookcovers from api
                var exBookCovers = await _quoteRepository.GetBookCoversAsync(quoteId);

                //old way for propertyBag
                //var propertyBag = new Tuple<Quote, IEnumerable<BookCover>>(quoteEntity, bookCovers);
                //var itsm1 = propertyBag.Item1;
                //var itsm2 = propertyBag.Item2;

                //way 2
                //(Quote book, IEnumerable<BookCover> bookCovers) propertyBag = (quoteEntity, bookCovers);

                //way 3 just return external model return and entity return to the QuoteWithBookCoversResultFilter to handle
                return Ok((quoteEntity, exBookCovers));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");

                //don't thorw everything into the front end, use for production
                //return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");

                //thorw everything into the front end
                throw;
            }
        }

        /// <summary>
        /// Create a new quote with specific samurai
        /// </summary>
        /// <param name="quoteCreationModel">The quote to create</param>
        /// <returns>An ActionResult of type Book</returns>
        /// /// <response code="422">Validation error</response>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity,
        //    Type = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary))]
        public async Task<ActionResult<QuoteModel>> CreateQuote([FromBody] QuoteCreationModel quoteCreationModel)
        {
            try
            {
                //fetch the samurai from the db for profle mapping to use
                var samurai = await _samuraiRepository.GetSamuraiAsync(quoteCreationModel.SamuraiId);
                if (samurai == null)
                {
                    return NotFound();
                }

                var quoteEntity = _mapper.Map<Quote>(quoteCreationModel);
                _quoteRepository.AddQuote(quoteEntity);

                await _quoteRepository.SaveChangeAsync();

                return CreatedAtRoute("GetQuote",
                    new { quoteId = quoteEntity.Id },
                    _mapper.Map<QuoteModel>(quoteEntity));


            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }

        ////add new quotes
        //[HttpPost("quotes")]
        //public async Task<IActionResult> PostNewQuotes(int id, [FromBody] List<string> quotes)
        //{
        //    try
        //    {
        //        //While EF is tracking the excisting object
        //        var query = _context.Samurais.Where(s => s.Id == id);
        //        var samurai = await query.FirstOrDefaultAsync();
        //        if (samurai == null)
        //        {
        //            return NotFound();
        //        }
        //        foreach (string quote in quotes)
        //        {
        //            samurai.Quotes.Add(new Quote
        //            {
        //                Text = quote
        //            });
        //        }

        //        //EF is not tracking excisting object
        //        //var quote = new Quote
        //        //{
        //        //    Text = "Now that I saved you, will you feed me dinner?",
        //        //    SamuraiId = id
        //        //};
        //        //await _context.Quotes.AddAsync(quote);

        //        await _context.SaveChangesAsync();
        //        return Ok();
        //    }
        //catch (Exception ex)
        //    {
        //        _logger.LogError($"{ex.Message}");
        //        throw;
        //    }
        //}
    }
}
