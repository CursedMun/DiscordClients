
using System;
using System.IO;

using DiscordClients.Console.Pages;
using DiscordClients.Helpers;

using EasyConsole;

namespace DiscordClients
{
    public class MainConsole : Program
    {

        public MainConsole(string[] args) : base("Менюшка", breadcrumbHeader: true)
        {
            GlobalVars.DataBase = new Core.SQL.SQLite(Path.Combine(AppContext.BaseDirectory, "data.db"));
            AddPage(new MainPage(this));
            AddPage(new PopulateDataBase(this));
            AddPage(new ExecuteBots(this));
            AddPage(new TestPage(this));
            if (args.Length > 0 && args[0] == "1")
                SetPage<ExecuteBots>();
            else
                SetPage<MainPage>();
        }

    }
}
