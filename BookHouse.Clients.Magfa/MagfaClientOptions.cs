using System;
using System.Collections.Generic;
using System.Text;

namespace BookHouse.Clients.Magfa
{
    public class MagfaClientOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string[] SenderNumber { get; set; }
    }
}
