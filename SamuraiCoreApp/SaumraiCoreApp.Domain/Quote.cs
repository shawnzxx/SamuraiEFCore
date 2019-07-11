using System;
using System.Collections.Generic;
using System.Text;

namespace SaumraiCoreApp.Domain
{
    public class Quote
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int SamuraiId { get; set; }
    }
}
