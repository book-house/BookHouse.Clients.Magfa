using BookHouse.Clients.Magfa.Resources;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Text.Json.Serialization;

namespace BookHouse.Clients.Magfa.Models
{
    public class MagfaBaseResult
    {
        public int Status { get; set; }

        //[JsonIgnore]
        //public string StatusDescription => MagfaStatus.ResourceManager.GetString(Status.ToString());
    }
}
