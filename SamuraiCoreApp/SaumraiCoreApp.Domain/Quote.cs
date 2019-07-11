using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SaumraiCoreApp.Domain
{
    [Table("Quotes")]
    public class Quote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Text { get; set; }

        public int SamuraiId { get; set; }
        public Samurai Samurai { get; set; }
    }
}
