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
    [Route("api/samurais")]
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
        public async Task<ActionResult<IEnumerable<SamuraiModel>>> GetSamurais()
        {
            try
            {
                var samuraiEntities = await _samuraiRepository.GetSamuraisAsync();
                return Ok(_mapper.Map<IEnumerable<SamuraiModel>>(samuraiEntities));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }

        // GET: api/samurais/5
        [HttpGet("{samuraiId}")]
        public async Task<ActionResult<SamuraiModel>> GetSamurai(int samuraiId)
        {
            try
            {
                var samurai = await _samuraiRepository.GetSamuraiAsync(samuraiId);
                if (samurai == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<SamuraiModel>(samurai));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }

        }

        // POST: api/Samurais
        [HttpPost]
        public async Task<ActionResult<SamuraiOnlyModel>> CreateSamurai([FromBody]SamuraiCreationModel input)
        {
            try
            {
                var samurai = new Samurai
                {
                    Name = input.Name
                };

                _context.Samurais.Add(samurai);
                await _context.SaveChangesAsync();

                return Ok(_mapper.Map<SamuraiOnlyModel>(samurai));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }

        private async Task SingleInsert()
        {
            var samurai = new Samurai { Name = "Shawnzxx" };
            this._context.Samurais.Add(samurai);
            await _context.SaveChangesAsync();
        }

        // PUT: api/Samurais/5
        [HttpPut("{samuraiId}")]
        public async Task<IActionResult> UpdateSamurai(int samuraiId, [FromBody] SamuraiUpdateModel samuraiForUpdate)
        {
            try
            {
                var samuraiEntity = await _samuraiRepository.GetSamuraiAsync(samuraiId);
                if (samuraiEntity == null)
                {
                    return NotFound();
                }

                _mapper.Map(samuraiForUpdate, samuraiEntity);

                //// update & save
                _samuraiRepository.UpdateSamurai(samuraiEntity);
                await _samuraiRepository.SaveChangesAsync();

                // return the SamuraiUpdateModel
                return Ok(_mapper.Map<SamuraiUpdateModel>(samuraiEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }
        

        /// <summary>
        /// Other legacy actions, clean up later
        /// </summary>
        /// <returns></returns>
        // PUT: api/Samurais
        [HttpPut()]
        public async Task<IActionResult> UpdateSamurais()
        {
            try
            {
                var samurais = _context.Samurais.ToList();
                foreach (var s in samurais)
                {
                    s.Name += "_1";
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }

        // DELETE: api/Samurais/01
        //Need to change to logical delete
        [HttpDelete("{samuraiId}")]
        public async Task<IActionResult> DeleteSamurai(int samuraiId)
        {
            try
            {
                var samuraiEntity = await _samuraiRepository.GetSamuraiAsync(samuraiId);
                if (samuraiEntity == null)
                {
                    return NotFound();
                }

                _context.Samurais.Remove(samuraiEntity);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }

        // Delete: api/Samurais?str=baba
        [HttpDelete()]
        public async Task<IActionResult> DeleteSamurais([FromQuery]string str)
        {
            try
            {
                var samurais = _context.Samurais.Where(s => s.Name.Contains(str));
                _context.Samurais.RemoveRange(samurais);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }
    }
}
