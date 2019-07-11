using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Contexts;
using WebApi.Entities;
using WebApi.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/samurais/{samuraiId}/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {

        private readonly SamuraiContext _context;
        private readonly ILogger<SamuraisController> _logger;
        private readonly IQuoteRepository _quoteRepository;

        public QuotesController(SamuraiContext context, ILogger<SamuraisController> logger, IQuoteRepository quoteRepository)
        {
            this._context = context;
            this._logger = logger;
            this._quoteRepository = quoteRepository
                ?? throw new ArgumentNullException(nameof(quoteRepository));
        }

        // GET: api/samurais/2/quotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes(int samuraiId)
        {
            try
            {
                var quotes = await _quoteRepository.GetQuotesAsync(samuraiId);
                return Ok(quotes);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");
            }
        }

        // GET: api/samurais/2/quotes/2
        [HttpGet("{quoteId}")]
        public async Task<ActionResult<Samurai>> GetSamuraiById(int samuraiId, int quoteId)
        {
            try
            {
                var quite = await _quoteRepository.GetQuoteAsync(samuraiId, quoteId);
                if (quite == null)
                {
                    return NotFound();
                }
                return Ok(quite);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");
            }

        }
    }
}
