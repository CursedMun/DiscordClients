using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace DiscordClients.Core.SQL.Tables
{
    public class ChannelType : SqlDocument
    {
        public string ChannelID { get; set; }
        public bool IsChannel { get; set; }
        public bool SpecialChannel { get; set; }
        public List<Bot> Bots { get; set; }
        public Time Time { get; set; }
    }
    public class Time
    {
        [JsonProperty("joinTime")]
        public DateTime JoinTime { get; set; }
        [JsonProperty("leaveTime")]
        public DateTime LeaveTime { get; set; }
    }
}
