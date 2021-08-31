using System;
using System.Collections.Generic;

using DiscordClients.Core.SQL;
using DiscordClients.Helpers.Discord;

namespace DiscordClients.Helpers
{
    public static class GlobalVars
    {
        public const string ws_url = "wss://gateway.discord.gg/?v=8&encoding=json";
        public const string GuildID = "728716141802815539";
        public static SQLite DataBase;
        public static TimeSpan JoinDelay = TimeSpan.FromMinutes(1);
        public static TimeSpan ReJoindelay = TimeSpan.FromSeconds(30);
        public static Dictionary<string, List<Channel>> DiscChannels = new Dictionary<string, List<Channel>>();
        public static List<string> Statuses = new List<string>()
        {
            "VALORANT",
            "Rust",
            "Tom Clancy's Rainbow Six Siege",
            "Sea of Thieves",
            "PLAYERUNKNOWN'S BATTLEGROUNDS",
            "Call of Duty Warzone",
            "GTA V",
            "Garry's Mod",
            "Minecraft",
            "Dota 2",
            "Dead by Daylight",
            "Counter-Strike: Global Offensive",
            "Arma 3",
            "DayZ",
            "Fallout",
            "Paladins",
            "Rocket League",
            "Destiny 2",
            "Apex Legends"
        };
    }
}
