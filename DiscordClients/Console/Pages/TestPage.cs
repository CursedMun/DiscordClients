using DiscordClients.Core.SQL.Tables;
using DiscordClients.Helpers;

using EasyConsole;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordClients.Console.Pages
{
    class TestPage : MenuPage
    {
        private static List<ClientManager> ClientManagers = new List<ClientManager>();
        private static Program Prog;
        public TestPage(Program program) : base("start?", program,
                  new Option("1", () => Test()),
                  new Option("Нет", () => Test1()))
        {
            Prog = program;
        }
        public class mytest
        {
            public DateTime blabla { get; set; }
        }
        private static void Test1()
        {
            Output.WriteLine(ConsoleColor.Red, DateTime.Now.ToString("F"));
            Output.WriteLine(ConsoleColor.Red, DateTime.Now.ToString("f"));
            mytest serialize = JsonConvert.DeserializeObject<mytest>("{blabla:\"01/01/0001 01:57:00 +00:00\"}");
            Output.WriteLine(ConsoleColor.Red, serialize.blabla.ToString());
            Output.WriteLine(ConsoleColor.Red, DateTime.Now.ToString());
            Output.WriteLine(ConsoleColor.Red, DateTime.Now.ToString("g"));
            Output.WriteLine(ConsoleColor.Red, DateTime.Now.ToString("M/d/yyyy HH:mm"));
        }

        private static void Test()
        {
            var bot = GlobalVars.DataBase.FindOne<ChannelType>(x => x.ChannelID == "854986080410271745");
            ClientManagers.Add(new ClientManager(new Client(bot.Bots.First().Token, GlobalVars.GuildID, bot.ChannelID, bot.IsChannel, bot.SpecialChannel)));
        }
    }
}
