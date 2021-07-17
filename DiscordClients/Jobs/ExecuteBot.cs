using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DiscordClients.Console.Pages;
using DiscordClients.Core.SQL.Tables;
using DiscordClients.Helpers;

using Quartz;

namespace DiscordClients.Jobs
{
    public class ExecuteBot : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.MergedJobDataMap;
            var dic = (KeyValuePair<Time, List<Bot>>)dataMap["JoinBots"];
            for (int i = 0; i < dic.Value.Count; i++)
            {
                var bot = dic.Value.ElementAt(i);
                var client = ExecuteBots.ClientManagers.Find(x => x.Client.Token == bot.Token);
                await Task.Delay(GlobalVars.JoinDelay);
                client.StartAsync(bot.Token);

            }
        }
    }
}
