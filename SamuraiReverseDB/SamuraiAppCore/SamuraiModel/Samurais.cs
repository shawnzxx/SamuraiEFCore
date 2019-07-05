using System;
using System.Collections.Generic;

namespace SamuraiModel
{
    public partial class Samurais
    {
        public Samurais()
        {
            Quotes = new HashSet<Quotes>();
            SamuraiBattle = new HashSet<SamuraiBattle>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int BattleId { get; set; }

        public virtual SecretIdentity SecretIdentity { get; set; }
        public virtual ICollection<Quotes> Quotes { get; set; }
        public virtual ICollection<SamuraiBattle> SamuraiBattle { get; set; }
    }
}