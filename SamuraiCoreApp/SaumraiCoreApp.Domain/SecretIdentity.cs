using System;
using System.Collections.Generic;
using System.Text;

namespace SaumraiCoreApp.Domain
{
    public class SecretIdentity
    {
        public int Id { get; set; }
        public string RealName { get; set; }
        //foreigner key property
        public int SamuraiId { get; set; }
    }
}
