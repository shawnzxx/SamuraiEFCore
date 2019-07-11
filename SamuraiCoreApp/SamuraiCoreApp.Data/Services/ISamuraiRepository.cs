using SaumraiCoreApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiCoreApp.Data.Services
{
    public interface ISamuraiRepository : IDisposable
    {
        Task<IEnumerable<Samurai>> GetSamuraisAsync();
        Task<Samurai> GetSamuraiAsync(int samuraiId);
    }
}
