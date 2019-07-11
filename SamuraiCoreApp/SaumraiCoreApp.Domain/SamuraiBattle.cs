namespace SaumraiCoreApp.Domain
{
    //need this mapping table to link many to many relationship
    public class SamuraiBattle
    {
        public int SamuraiId { get; set; }

        public Samurai Samurai { get; set; }

        public int BattleId { get; set; }

        public Battle Battle { get; set; }

    }
}