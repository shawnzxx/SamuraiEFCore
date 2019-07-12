using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IQuoteRepository : IDisposable
    {
        //get all quotes in db
        Task<IEnumerable<Quote>> GetQuotesAsync();
        //get quotes collection by pasing ids array
        Task<IEnumerable<Quote>> GetQuotesAsync(IEnumerable<int> quoteIds);

        Task<Quote> GetQuoteAsync(int quoteId);
 
        void AddQuote(Quote quote);

        Task<bool> SaveChangeAsync();

    }
}
