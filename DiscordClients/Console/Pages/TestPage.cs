using System;
using System.Collections.Generic;
using System.Linq;

using DiscordClients.Core.SQL.Tables;
using DiscordClients.Helpers;

using EasyConsole;

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

        private static void Test1()
        {
            Output.WriteLine(ConsoleColor.Red, DateTime.Now.ToString());
        }

        private static void Test()
        {
            var bot = GlobalVars.DataBase.FindOne<ChannelType>(x => x.ChannelID == "854986080410271745");
            ClientManagers.Add(new ClientManager(new Client(bot.Bots.First().Token, GlobalVars.GuildID, bot.ChannelID, bot.IsChannel, bot.SpecialChannel)));
        }
    }
}
