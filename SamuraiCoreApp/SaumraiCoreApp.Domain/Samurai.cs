using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaumraiCoreApp.Domain
{
    [Table("Samurais")]
    public class Samurai
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        //one to many
        public List<Quote> Quotes { get; set; } = new List<Quote>();

        //many to many
        public List<SamuraiBattle> SamuraiBattles { get; set; }

        //navigation property: one to one
        public SecretIdentity SecretIdentity { get; set; }
    }
}