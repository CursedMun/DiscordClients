using DiscordClients.Console.Pages;
using DiscordClients.Core.SQL.Tables;
using DiscordClients.Helpers;

using Quartz;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordClients.Jobs
{
    public class FinishBot : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.MergedJobDataMap;
            var dic = (KeyValuePair<Time, List<Bot>>)dataMap["LeaveBots"];
            for (int i = 0; i < dic.Value.Count; i++)
            {
                var bot = dic.Value.ElementAt(i);
                var client = ExecuteBots.ClientsManagers.Find(x => x.Client.Token == bot.Token);
                await Task.Delay(GlobalVars.JoinDelay);
                client.Disconnect(bot.Token);
            }
        }
    }
}
