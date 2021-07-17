
using Newtonsoft.Json;

namespace DiscordClients.Helpers.Discord
{
    public partial class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("parent_id")]
        public string ParentId { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonIgnore]
        public int UsersIn { get; set; }
        [JsonProperty("user_limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? UserLimit { get; set; }
    }
}
