namespace WebApi.Entities
{
    //need this mapping table to link many to many relationship
    public class SamuraiBattle
    {
        public int SamuraiId { get; set; }
        //Navigation property
        public Samurai Samurai { get; set; }

        public int BattleId { get; set; }
        //Navigation property
        public Battle Battle { get; set; }

    }
}