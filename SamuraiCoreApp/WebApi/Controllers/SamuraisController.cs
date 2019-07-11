using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiCoreApp.Data;
using SamuraiCoreApp.Data.Services;
using SaumraiCoreApp.Domain;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController] //What is ApiController attribute: https://www.strathweb.com/2018/02/exploring-the-apicontrollerattribute-and-its-features-for-asp-net-core-mvc-2-1/
    public class SamuraisController : ControllerBase
    {
        private readonly SamuraiContext _context;
        private readonly ILogger<SamuraisController> _logger;
        private readonly ISamuraiRepository _samuraiRepository;

        public SamuraisController(SamuraiContext context, ILogger<SamuraisController> logger, ISamuraiRepository samuraiRepository)
        {
            this._context = context;
            this._logger = logger;
            this._samuraiRepository = samuraiRepository 
                ?? throw new ArgumentNullException(nameof(samuraiRepository));
        }

        // GET: api/Samurais
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Samurai>>> GetSamurais()
        {
            try
            {
                var samuraiEntities = await _samuraiRepository.GetSamuraisAsync();
                return Ok(samuraiEntities);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");
            }
        }

        // GET: api/Samurais/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Samurai>> GetSamuraiById(int id)
        {
            try
            {
                var result = await _samuraiRepository.GetSamuraiAsync(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");
            }
            
        }

        // POST: api/Samurais
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SamuraiModel input)
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
        public async Task<IActionResult> PostNewQuotes(int id, [FromBody] List<string> quotes)
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
        public async Task<IActionResult> DeleteMany([FromQuery]string str)
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
