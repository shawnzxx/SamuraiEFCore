using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class QuoteWithCoversModel : QuoteModel
    {
        public IEnumerable<BookCoverModel> QuoteCovers { get; set; }
    }
}
