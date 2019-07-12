﻿using System;
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
using WebApi.Filters;
using WebApi.Models;
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
        private readonly IMapper _mapper;

        public QuotesController(SamuraiContext context, 
            ILogger<SamuraisController> logger, 
            IQuoteRepository quoteRepository,
            IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/samurais/2/quotes
        [HttpGet]
        [QuotesResultFilter]
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
        [HttpGet]
        [QuoteResultFilter]
        [Route("{quoteId}", Name ="GetQuote")]
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

        [HttpPost]
        public async Task<IActionResult> CreateQuote ([FromBody] QuoteCreationModel quote)
        {
            try
            {
                var quoteEntity = _mapper.Map<Quote>(quote);
                _quoteRepository.AddQuote(quoteEntity);

                await _quoteRepository.SaveChangeAsync();

                //fetch the samurai from the db for profle mapping to use
                var samurai = await _context.Samurais.FirstOrDefaultAsync(s => s.Id == quoteEntity.SamuraiId);

                return CreatedAtRoute("GetQuote", 
                    new { samuraiId = quoteEntity.SamuraiId, quoteId = quoteEntity.Id },
                    _mapper.Map<QuoteOutPutModel>(quoteEntity));
                
                
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");
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
        //    catch (Exception)
        //    {
        //        return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");
        //    }
        //}
    }
}
