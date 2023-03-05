namespace BookHouse.Clients.Magfa.Models
{
    public class MagfaBaseResult
    {
        public int Status { get; set; }

        //[JsonIgnore]
        //public string StatusDescription => MagfaStatus.ResourceManager.GetString(Status.ToString());
    }
}