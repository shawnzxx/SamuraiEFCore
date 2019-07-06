using SaumraiCoreApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class SamuraiModel
    {
        public string Name { get; set; }
        public List<Quote> Quotes { get; set; }
    }
}
