
using System;
using System.Collections.Generic;


namespace DiscordClients.Core.SQL.Tables
{
    public class Bot : SqlDocument
    {
        public string Token { get; set; }

        public List<DateTime> JoinedAT { get; set; }
        public List<DateTime> LeftAT { get; set; }

        public DateTime TotalTime { get; set; }
    }
}
