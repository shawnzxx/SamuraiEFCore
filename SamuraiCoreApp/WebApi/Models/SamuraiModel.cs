using WebApi.Entities;
using System.Collections.Generic;

namespace WebApi.Models
{
    /// <summary>
    /// A samurai with Id, Name, total quote counts and list of QuoteModel
    /// </summary>
    public class SamuraiModel
    {
        /// <summary>
        /// The id of the samurai
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the samurai
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Total quotes counts belong to the samurai
        /// </summary>
        public int QuoteCounts { get; set; }

        /// <summary>
        /// Navigation property link to all QuoteModel
        /// </summary>
        public List<QuoteModel> Quotes { get; set; }
    }
}
