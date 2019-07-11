﻿namespace WebApi.Entities
{
    public class SecretIdentity
    {
        public int Id { get; set; }

        public string RealName { get; set; }

        //foreigner key property
        public int SamuraiId { get; set; }
        //Navigation property
        public Samurai Samurai { get; set; }
    }
}
