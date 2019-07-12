using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Contexts;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Services
{
    public class QuoteRepository : IQuoteRepository
    {
        private SamuraiContext _context;

        public QuoteRepository(SamuraiContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<Quote>> GetQuotesAsync(int samuraiId)
        {
            return await _context.Quotes
                .Include(q => q.Samurai)
                .Where(q => q.SamuraiId == samuraiId)
                .ToListAsync();
        }

        public async Task<Quote> GetQuoteAsync(int samuraiId, int quoteId)
        {
            return await _context.Quotes
                .Include(q => q.Samurai)
                .Where(q => q.SamuraiId == samuraiId && q.Id == quoteId)
                .FirstOrDefaultAsync();
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

        public void AddQuote(Quote quote)
        {
            if (quote == null) {
                throw new ArgumentNullException(nameof(quote));
            }

            _context.Add(quote);
        }

        public async Task<bool> SaveChangeAsync()
        {
            //return true if 1 or more entities were changed
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
