using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiCoreApp.Data;
using SaumraiCoreApp.Domain;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SamuraisController : ControllerBase
    {
        private readonly SamuraiContext _context;
        private readonly ILogger<SamuraisController> _logger;

        public SamuraisController(SamuraiContext context, ILogger<SamuraisController> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        // GET: api/Samurais
        [HttpGet]
        public async Task<ActionResult<Samurai[]>> GetSamurais()
        {
            try
            {
                //Inculde you can't do filtering
                //var samuraiWithQuotes = await _context.Samurais.Include(s => s.Quotes).ToListAsync();

                //use projection for selected fields
                //var samuraiWithHappyQuote = await _context.Samurais
                //    .Select(s => new
                //    {
                //        s.Id,
                //        s.Name,
                //        HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
                //    })
                //    .ToListAsync();
                //var result = samuraiWithHappyQuote;

                //Filtering children currentlly requires multiple queries:
                //do two queries, 2nd one filtering out the none happy quote, EF can figure out realted DBSet inside the memory
                //var samurais = _context.Samurais.ToList();
                //var happyQuotes = _context.Quotes.Where(q => q.Text.Contains("happy")).ToList();

                //only get the samurai with happy quotes
                var samurais = await _context.Samurais
                    .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
                    .ToListAsync();

                return Ok(samurais);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");
            }
        }

        // GET: api/Samurais/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Samurai>> GetById(int id, bool simplify = false)
        {
            dynamic result = null;
            //_logger.LogWarning("Hello World In Values Controller");
            if (simplify)
            {
                
                //some properties with quote count
                //result = await _context.Samurais
                //    .Where(s => s.Id == id)
                //    .Select(s => new { s.Name, s.Quotes.Count, s.Quotes })
                //    .FirstOrDefaultAsync();

                //select projection filtering
                result = await _context.Samurais
                    .Where(s => s.Id == id)
                    .Select(s => new { s.Name, TotalQuotes = s.Quotes.Count, HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))})
                    .FirstOrDefaultAsync();
                    
            }
            else {
                //You can eager load related data or load after the fact
                result = _context.Samurais
                    .Where(s => s.Id == id)
                    .Include(s => s.Quotes)
                    .FirstOrDefaultAsync();
            }

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST: api/Samurais
        [HttpPost]
        public async Task<IActionResult> Post(SamuraiModel input)
        {
            try
            {
                var samurai = new Samurai {
                    Name = input.Name,
                    Quotes = input.Quotes
                };

                _context.Samurais.Add(samurai);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");
            }
        }

        //add new quotes
        [HttpPost("{id}/quotes")]
        public async Task<IActionResult> PostNewQuotes(int id, List<string> quotes)
        {
            try
            {
                //While EF is tracking the excisting object
                var query = _context.Samurais.Where(s => s.Id == id);
                var samurai = await query.FirstOrDefaultAsync();
                if (samurai == null)
                {
                    return NotFound();
                }
                foreach (string quote in quotes)
                {
                    samurai.Quotes.Add(new Quote
                    {
                        Text = quote
                    });
                }

                //EF is not tracking excisting object
                //var quote = new Quote
                //{
                //    Text = "Now that I saved you, will you feed me dinner?",
                //    SamuraiId = id
                //};
                //await _context.Quotes.AddAsync(quote);

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");
            }
        }

        private async Task SingleInsert()
        {
            var samurai = new Samurai { Name = "Shawnzxx" };
            this._context.Samurais.Add(samurai);
            await _context.SaveChangesAsync();
        }

        // PUT: api/Samurais/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SamuraiModel input)
        {
            var query = _context.Samurais.Where(s => s.Id == id);
            var result = await query.FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }
            result.Name = input.Name;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // PUT: api/Samurais
        [HttpPut()]
        public async Task<IActionResult> PutAll()
        {
            var samurais = _context.Samurais.ToList();
            foreach (var s in samurais)
            {
                s.Name += "_1";
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Delete: api/Samurais?str=baba
        [HttpDelete()]
        public async Task<IActionResult> DeleteMany(string str)
        {
            var samurais = _context.Samurais.Where(s => s.Name.Contains(str));
            _context.Samurais.RemoveRange(samurais);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Samurais/01
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var query = _context.Samurais.Where(s => s.Id == id);
            var result = await query.FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }

            _context.Samurais.Remove(result);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
