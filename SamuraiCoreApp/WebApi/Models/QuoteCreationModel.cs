namespace WebApi.Models
{
    /// <summary>
    /// Quote creation front end input model
    /// </summary>
    public class QuoteCreationModel
    {
        /// <summary>
        /// Samurai id you want to create the quote in
        /// </summary>
        public int SamuraiId { get; set; }
        /// <summary>
        /// Quote test you want to create
        /// </summary>
        public string Text { get; set; }
    }
}
