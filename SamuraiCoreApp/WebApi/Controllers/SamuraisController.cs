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
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController] //What is ApiController attribute: https://www.strathweb.com/2018/02/exploring-the-apicontrollerattribute-and-its-features-for-asp-net-core-mvc-2-1/
    public class SamuraisController : ControllerBase
    {
        private readonly SamuraiContext _context;
        private readonly ILogger<SamuraisController> _logger;
        private readonly ISamuraiRepository _samuraiRepository;
        private readonly IMapper _mapper;

        public SamuraisController(SamuraiContext context, 
            ILogger<SamuraisController> logger, 
            ISamuraiRepository samuraiRepository,
            IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _samuraiRepository = samuraiRepository ?? throw new ArgumentNullException(nameof(samuraiRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/samurais
        [HttpGet]
        public async Task<IActionResult> GetSamurais()
        {
            try
            {
                var samuraiEntities = await _samuraiRepository.GetSamuraisAsync();
                return Ok(_mapper.Map<SamuraiOutputModel[]>(samuraiEntities));
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/samurais/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSamuraiById(int id)
        {
            try
            {
                var samurai = await _samuraiRepository.GetSamuraiAsync(id);
                if (samurai == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<SamuraiOutputModel>(samurai));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");
            }
            
        }

        // POST: api/Samurais
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SamuraiCreationModel input)
        {
            try
            {
                var samurai = new Samurai {
                    Name = input.Name
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

        private async Task SingleInsert()
        {
            var samurai = new Samurai { Name = "Shawnzxx" };
            this._context.Samurais.Add(samurai);
            await _context.SaveChangesAsync();
        }

        // PUT: api/Samurais/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SamuraiCreationModel input)
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
