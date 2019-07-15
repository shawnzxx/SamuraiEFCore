using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    /// <summary>
    /// Quote with its covers front end output model
    /// </summary>
    public class QuoteWithCoversModel : QuoteModel
    {
        /// <summary>
        /// Covers list belong to the quote
        /// </summary>
        public IEnumerable<BookCoverModel> QuoteCovers { get; set; }
    }
}
