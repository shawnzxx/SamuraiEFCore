using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    /// <summary>
    /// samurai update front end input model
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
