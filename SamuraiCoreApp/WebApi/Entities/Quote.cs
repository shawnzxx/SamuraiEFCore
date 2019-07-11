using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    [Table("Quotes")]
    public class Quote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Text { get; set; }

        //Foreign key back to Samurais Table
        public int SamuraiId { get; set; }
        //Navigation property for code easy navigate back t0 Samuai object
        public Samurai Samurai { get; set; }
    }
}
