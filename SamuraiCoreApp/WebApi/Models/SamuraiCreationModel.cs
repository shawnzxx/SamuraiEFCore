using System.Collections.Generic;
using WebApi.Entities;

namespace WebApi.Models
{
    public class SamuraiCreationModel
    {
        public string Name { get; set; }

        public List<Quote> Quotes { get; set; }
    }
}
