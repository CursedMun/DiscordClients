
using EasyConsole;

namespace DiscordClients.Console.Pages
{
    public class MainPage : MenuPage
    {
        public static bool StartedBots = false;
        public MainPage(Program program) : base("Главное Меню", program,
                  new Option("Запустить Ботов", () =>
                  {
                      if (!StartedBots) program.NavigateTo<ExecuteBots>(); else program.NavigateHome();
                  }
                  ),
                  new Option("Заполнить бд", () => program.NavigateTo<PopulateDataBase>()),
                  new Option("test", () => program.NavigateTo<TestPage>()))
        {
            System.Console.Clear();
        }


    }

}
