using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public SamuraisController(SamuraiContext context)
        {
            this._context = context;
        }

        // GET: api/Samurais
        [HttpGet]
        public async Task<ActionResult<Samurai[]>> GetSamurais(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var results = await _context.Samurais.FirstOrDefaultAsync(s => s.Name == name);
                    return Ok(results);
                }
                else {
                    var results = await _context.Samurais.ToArrayAsync();
                    return Ok(results);
                }
                
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failed");
            }
        }

        // GET: api/Samurais/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Samurai>> GetById(int id)
        {
            var query = _context.Samurais.Where(s => s.Id == id);
            var result = await query.FirstOrDefaultAsync();
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
                var samurai = new Samurai { Name = input.Name };

                var battle = new Battle
                {
                    Name = input.BattleName,
                    StartDate = DateTime.Parse(input.BattleStartDate),
                    EndDate = DateTime.Parse(input.BattleEndDate),
                };

                _context.AddRange(samurai, battle);
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
