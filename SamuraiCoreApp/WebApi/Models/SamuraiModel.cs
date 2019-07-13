using WebApi.Entities;
using System.Collections.Generic;

namespace WebApi.Models
{
    public class SamuraiModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int QuoteCounts { get; set; }

        public List<QuoteModel> Quotes { get; set; }
    }
}
