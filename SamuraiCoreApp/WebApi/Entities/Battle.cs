﻿using System;
using System.Collections.Generic;

namespace WebApi.Entities
{
    public class Battle
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        //public List<Samurai> Samurais { get; set; }
        public List<SamuraiBattle> SamuraiBattles { get; set; }
    }
}
