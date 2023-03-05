namespace BookHouse.Clients.Magfa.Models
{
    public class MagfaSendResult : MagfaBaseResult
    {
        public MagfaMessage[] Messages { get; set; }
    }

    public class MagfaMessage : MagfaBaseResult
    {
        public long? UserId { get; set; }
        public string Recipient { get; set; }
        public long? Id { get; set; }
        public int? Parts { get; set; }
        public float? Tariff { get; set; }
        public string Alphabet { get; set; }
    }
}