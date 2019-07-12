using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Contexts;
using WebApi.Entities;

namespace WebApi.Services
{
    public class SamuraiRepository : ISamuraiRepository
    {
        private SamuraiContext _context;

        public SamuraiRepository(SamuraiContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<Samurai>> GetSamuraisAsync()
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
            //var samurais = await _context.Samurais
            //    .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
            //    .ToListAsync();
            return await _context.Samurais
                .Include(s=>s.Quotes)
                .ToListAsync();
        }

        public async Task<Samurai> GetSamuraiAsync(int id)
        {
            //some properties with quote count
            //result = await _context.Samurais
            //    .Where(s => s.Id == id)
            //    .Select(s => new { s.Name, s.Quotes.Count, s.Quotes })
            //    .FirstOrDefaultAsync();

            //select projection filtering
            //result = await _context.Samurais
            //    .Where(s => s.Id == id)
            //    .Select(s => new { s.Name, TotalQuotes = s.Quotes.Count, HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy")) })
            //    .FirstOrDefaultAsync();

            return await _context.Samurais
                .Include(s=>s.Quotes)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
    }
}
