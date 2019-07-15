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
    [Produces("application/json", "application/xml")] //Produce define Accept header type
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

        /// <summary>
        /// Get all samurai collections
        /// </summary>
        /// <returns>Return all list of Samurais</returns>
        /// <response code="200">Return all list of Samurais</response>
        // GET: api/samurais
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        /// <summary>
        /// Get Samurai by his/her samuraiId
        /// </summary>
        /// <param name="samuraiId">The id of samurai you want to get</param>
        /// <returns>An ActionResult of type SamuraiModel</returns>
        /// <response code="200">Return the requested Samurai</response>
        // GET: api/samurais/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{samuraiId}", Name = "GetSamurai")]
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

        /// <summary>
        /// Create a new samurai with his/her name
        /// </summary>
        /// <param name="samuraiCreationModel">passing in SamuraiCreationModel</param>
        /// <returns>Return created Samurai</returns>
        /// <response code="201">Return created Samurai</response>
        // POST: api/Samurais
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<SamuraiModel>> CreateSamurai([FromBody]SamuraiCreationModel samuraiCreationModel)
        {
            try
            {
                var samuraiEntity = _mapper.Map<Samurai>(samuraiCreationModel);

                _context.Samurais.Add(samuraiEntity);
                await _context.SaveChangesAsync();

                return CreatedAtRoute("GetSamurai",
                    new { samuraiId = samuraiEntity.Id },
                    _mapper.Map<SamuraiModel>(samuraiEntity));
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

        /// <summary>
        /// Update sumurai name by using samuraiId
        /// </summary>
        /// <param name="samuraiId">The id of samurai you want to update</param>
        /// <param name="samuraiForUpdate">SamuraiUpdateModel json object you want to update</param>
        /// <returns>Return updated Samurai</returns>
        /// <response code="200">Return updated Samurai</response>
        // PUT: api/Samurais/5
        [HttpPut("{samuraiId}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<SamuraiModel>> UpdateSamurai(int samuraiId, [FromBody] SamuraiUpdateModel samuraiForUpdate)
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
                return Ok(_mapper.Map<SamuraiModel>(samuraiEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }
        

        /// <summary>
        /// Legacy actions: Update list of Samurais
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

        /// <summary>
        /// Legacy actions: Delete Samurai by it's samuraiId
        /// </summary>
        /// <param name="samuraiId">Id of Samurai</param>
        /// <returns>200 sucess</returns>
        // DELETE: api/Samurais/01
        //Need to change to logical delete
        [HttpDelete("{samuraiId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
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

        /// <summary>
        /// Legacy actions: Delete list of Samurais by passed in string which Samurai's name contained
        /// </summary>
        /// <param name="str">String match pattern which will matched with Samurai's name</param>
        /// <returns>200 sucess</returns>
        // Delete: api/Samurais?str=baba
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete()]
        public async Task<IActionResult> DeleteSamurais([FromQuery]string str)
        {
            try
            {
                var samurais = _context.Samurais.Where(s => s.Name.Contains(str));
                if (samurais == null) {
                    return NotFound();
                }
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
