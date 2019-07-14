using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    /// <summary>
    /// A samurai for update with Name field
    /// </summary>
    public class SamuraiUpdateModel
    {
        /// <summary>
        /// The name of the Samurai
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
