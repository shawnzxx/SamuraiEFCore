using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    /// <summary>
    /// Simple samurai front end output model
    /// </summary>
    public class SamuraiOnlyModel
    {
        /// <summary>
        /// Id of the samurai
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the samurai
        /// </summary>
        public string Name { get; set; }
    }
}
