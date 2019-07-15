using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    /// <summary>
    /// Quote front end output model
    /// </summary>
    public class QuoteModel
    {
        /// <summary>
        /// The id of the quote
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The text of the quote
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// The name of sumurai which linked to the current quote 
        /// </summary>
        public string SamuraiName { get; set; }
    }
}
