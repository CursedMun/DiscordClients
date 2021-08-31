
using Newtonsoft.Json;

namespace DiscordClients.Helpers
{
    public class DiscordEvent
    {
        [JsonProperty("t")]
        public string T { get; set; }

        [JsonProperty("s")]
        public object S { get; set; }

        [JsonProperty("op")]
        public int Op { get; set; }

        [JsonProperty("d")]
        public D D { get; set; }
    }
    public class D
    {
        public int? heartbeat_interval { get; set; }
        [JsonProperty("user_id")]
        public string? UserId { get; set; }

        [JsonProperty("guild_id")]
        public string? GuildId { get; set; }

        [JsonProperty("channel_id")]
        public string? ChannelId { get; set; }
        [JsonProperty("user")]
        public User? User { get; set; }
    }
    public partial class User
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
