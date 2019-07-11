using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;

namespace WebApi.Services
{
    public interface IQuoteRepository : IDisposable
    {
        Task<IEnumerable<Quote>> GetQuotesAsync(int samuraiId);
        Task<Quote> GetQuoteAsync(int samuraiId, int quoteId);
    }
}
