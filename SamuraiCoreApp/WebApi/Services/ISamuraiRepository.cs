using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities;

namespace WebApi.Services
{
    public interface ISamuraiRepository : IDisposable
    {
        Task<IEnumerable<Samurai>> GetSamuraisAsync();
        Task<Samurai> GetSamuraiAsync(int samuraiId);
    }
}
