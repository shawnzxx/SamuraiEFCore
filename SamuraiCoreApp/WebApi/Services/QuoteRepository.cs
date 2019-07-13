using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApi.Contexts;
using WebApi.Entities;
using WebApi.ExternalModels;
using WebApi.Models;

namespace WebApi.Services
{
    public class QuoteRepository : IQuoteRepository
    {
        private SamuraiContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public QuoteRepository(SamuraiContext context, IHttpClientFactory httpClientFactory)
        {
            this._context = context;
            this._httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<Quote>> GetQuotesAsync()
        {
            return await _context.Quotes
                .Include(q => q.Samurai)
                .ToListAsync();
        }

        public async Task<Quote> GetQuoteAsync(int id)
        {
            return await _context.Quotes
                .Include(q => q.Samurai)
                .FirstOrDefaultAsync(q => q.Id == id);
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

        public async Task<IEnumerable<Quote>> GetQuotesAsync(IEnumerable<int> quoteIds)
        {
            return await _context.Quotes.Where(q => quoteIds.Contains(q.Id))
                .Include(q => q.Samurai).ToListAsync();
        }

        public IEnumerable<Quote> GetQuotes()
        {
            //delay database query 2 seconds to test sync concurrent call
            _context.Database.ExecuteSqlCommand("WAITFOR DELAY '00:00:02';");

            return _context.Quotes.Include(q => q.Samurai).ToList();
        }

        public async Task<BookCover> GetBookCoverTestAsync(string text)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"https://localhost:5001/api/bookcovers/{text}");

            if (response.IsSuccessStatusCode) {
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<BookCover>(responseJson);
            }
            return null;
        }

        public async Task<IEnumerable<BookCover>> GetBookCoversAsync(int quoteId)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var bookCovers = new List<BookCover>();

            //create a list of fake bookcovers request urls
            var bookCoverUrls = new[]
            {
                $"https://localhost:5001/api/bookcovers/{quoteId}-dummycover1",
                $"https://localhost:5001/api/bookcovers/{quoteId}-dummycover2",
                $"https://localhost:5001/api/bookcovers/{quoteId}-dummycover3",
                $"https://localhost:5001/api/bookcovers/{quoteId}-dummycover4",
                $"https://localhost:5001/api/bookcovers/{quoteId}-dummycover5"
            };

            foreach (var url in bookCoverUrls)
            {
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode) {
                    var json = await response.Content.ReadAsStringAsync();
                    var bookCover = JsonConvert.DeserializeObject<BookCover>(json);
                    bookCovers.Add(bookCover);
                }
            }

            return bookCovers;
        }
    }
}
