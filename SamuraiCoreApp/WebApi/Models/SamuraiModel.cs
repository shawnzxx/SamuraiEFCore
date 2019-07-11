using System.Collections.Generic;
using WebApi.Entities;

namespace WebApi.Models
{
    public class SamuraiModel
    {
        public string Name { get; set; }
        public List<Quote> Quotes { get; set; }
    }
}
