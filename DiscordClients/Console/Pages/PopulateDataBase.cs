using DiscordClients.Core.SQL.Tables;
using DiscordClients.Helpers;

using EasyConsole;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DiscordClients.Console.Pages
{
    public class PopulateDataBase : Page
    {
        public PopulateDataBase(Program program) : base("Input", program)
        {

        }
        private partial class BotsJson
        {
            [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Ignore)]
            public string ChannelId { get; set; }

            [JsonProperty("tokens")]
            public string[] Tokens { get; set; }

            [JsonProperty("category_id", NullValueHandling = NullValueHandling.Ignore)]
            public string CategoryId { get; set; }
            [JsonProperty("special", NullValueHandling = NullValueHandling.Ignore)]
            public bool Special { get; set; }
            [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
            public Time Time { get; set; }
        }
        public override void Display()
        {
            base.Display();
            Output.WriteLine(ConsoleColor.Green, "Ожидайте...");
            List<BotsJson> Bots = JsonConvert.DeserializeObject<List<BotsJson>>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Bots.json")));
            GlobalVars.DataBase.DeleteAll<ChannelType>();
            var cultureOfMyDates = CultureInfo.GetCultureInfo("ru");
            for (int i = 0; i < Bots.Count; i++)
            {
                var bot = Bots.ElementAt(i);
                //var s = GlobalVars.DataBase.FindOne<ChannelType>(x => x.ChannelID == (bot.ChannelId ?? bot.CategoryId));
                var channel = new ChannelType()
                {
                    Id = null,
                    ChannelID = bot.ChannelId ?? bot.CategoryId,
                    IsChannel = bot.ChannelId != null,
                    SpecialChannel = bot.Special,
                    Time = bot.Time,
                    Bots = new List<Bot>()
                };
                for (int x = 0; x < bot.Tokens.Length; x++)
                {
                    var token = bot.Tokens.ElementAt(x);
                    var botexists = channel.Bots.Find(x => x.Token == token);
                    if (botexists != null)
                        continue;

                    channel.Bots.Add(new Bot() { Token = token });


                }
                Output.WriteLine(ConsoleColor.Green, $@"----Добавлен:----
                                                          Канал:{channel.Id}
                                                          Количество ботов:{channel.Bots.Count}
                                                          Заход:{channel.Time.JoinTime.ToString()} 
                                                          Выход:{channel.Time.LeaveTime.ToString()}
                                                       ------------------");
                GlobalVars.DataBase.Upsert(channel);
            }
            Input.ReadString("Press [Enter] to navigate home");
            Program.NavigateHome();
        }
    }
}
