using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
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
        private IHttpClientFactory _httpClientFactory;
        private readonly ILogger<QuoteRepository> _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public QuoteRepository(SamuraiContext context, IHttpClientFactory httpClientFactory, ILogger<QuoteRepository> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cancellationTokenSource = new CancellationTokenSource();
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
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
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
                $"https://localhost:6001/api/bookcovers/{quoteId}-dummycover1",
                //$"https://localhost:6001/api/bookcovers/{quoteId}-dummycover2?returnFault=true",
                $"https://localhost:6001/api/bookcovers/{quoteId}-dummycover3",
                $"https://localhost:6001/api/bookcovers/{quoteId}-dummycover4",
                $"https://localhost:6001/api/bookcovers/{quoteId}-dummycover5"
            };

            //Try contain bridge network
            //var bookCoverUrls = new[]
            //{
            //    $"http://bookcoversapi:8081/api/bookcovers/{quoteId}-dummycover1",
            //    $"http://bookcoversapi:8081/api/bookcovers/{quoteId}-dummycover2",
            //    $"http://bookcoversapi:8081/api/bookcovers/{quoteId}-dummycover3",
            //    $"http://bookcoversapi:8081/api/bookcovers/{quoteId}-dummycover4",
            //    $"http://bookcoversapi:8081/api/bookcovers/{quoteId}-dummycover5"
            //};

            //execute tasks one by one in order
            //foreach (var bookCoverUrl in bookCoverUrls)
            //{
            //    var response = await httpClient
            //       .GetAsync(bookCoverUrl);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        bookCovers.Add(JsonConvert.DeserializeObject<BookCover>(
            //            await response.Content.ReadAsStringAsync()));
            //    }
            //}

            //execute tasks in parallel
            //create tasks, task haven't start yet, this is IEnumerable<Task<BookCover>> type
            var downloadBookCoverTasksQuery =
                from bookCoverUrl
                in bookCoverUrls
                select DownloadBookCoverAsync(httpClient, bookCoverUrl, _cancellationTokenSource.Token);

            //start tasks, this is List<Task<BookCover>> type
            //var downloadBookCoverTasks = downloadBookCoverTasksQuery.ToList();

            //or start tasks without convert to list
            //use which one? https://stackoverflow.com/questions/3628425/ienumerable-vs-list-what-to-use-how-do-they-work
            try
            {
                return await Task.WhenAll(downloadBookCoverTasksQuery);
            }
            //based on cancellation exception we can log cancel reason and task status in detail
            catch (OperationCanceledException operationCanceledException)
            {
                _logger.LogInformation($"{operationCanceledException.Message}");
                foreach (var task in downloadBookCoverTasksQuery)
                {
                    _logger.LogInformation($"Task {task.Id} has status {task.Status}");
                }

                //return empty list of BookCover
                return new List<BookCover>();
            }
            catch (Exception exception) {
                _logger.LogError($"{exception.Message}");
                throw;
            }
        }

        private async Task<BookCover> DownloadBookCoverAsync(HttpClient httpClient, string bookCoverUrl, CancellationToken cancellationToken)
        {
            //throw new Exception("Cannot download book cover, writer isn't finishing book fast enough.");

            var response = await httpClient.GetAsync(bookCoverUrl, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bookCover = JsonConvert.DeserializeObject<BookCover>(json);
                return bookCover;
            }
            //if any task failed cancel tasks to free up threads
            _cancellationTokenSource.Cancel();
            
            return null;
            
        }
    }
}
