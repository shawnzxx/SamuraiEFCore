using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.ExternalModels;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IQuoteRepository : IDisposable
    {
        IEnumerable<Quote> GetQuotes();

        //get all quotes in db
        Task<IEnumerable<Quote>> GetQuotesAsync();
        //get quotes collection by pasing ids array
        Task<IEnumerable<Quote>> GetQuotesAsync(IEnumerable<int> quoteIds);
        //get quote by id
        Task<Quote> GetQuoteAsync(int quoteId);
 
        void AddQuote(Quote quote);
        Task<bool> SaveChangeAsync();

        Task<BookCover> GetBookCoverTestAsync(string anyText);

        Task<IEnumerable<BookCover>> GetBookCoversAsync(int quoteId);

    }
}
