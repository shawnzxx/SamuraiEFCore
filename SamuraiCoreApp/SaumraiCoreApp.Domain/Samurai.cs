using System.Collections.Generic;

namespace SaumraiCoreApp.Domain
{
    public class Samurai
    {
        public Samurai()
        {
            Quotes = new List<Quote>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        //one to many
        public List<Quote> Quotes { get; set; }

        //many to many
        public List<SamuraiBattle> SamuraiBattles { get; set; }
        //navigation property: one to one
        public SecretIdentity SecretIdentity { get; set; }
    }
}