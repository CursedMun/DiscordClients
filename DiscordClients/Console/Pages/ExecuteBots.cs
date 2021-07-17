using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using ConsoleTables;

using DiscordClients.Core.SQL.Tables;
using DiscordClients.Helpers;
using DiscordClients.Helpers.Discord;
using DiscordClients.Jobs;

using EasyConsole;

using Newtonsoft.Json;

using Quartz;
using Quartz.Impl;

namespace DiscordClients.Console.Pages
{
    public class ExecuteBots : Page
    {
        public static List<ClientManager> ClientManagers;
        public static Dictionary<Time, List<Bot>> Dict;
        private static Program Prog;
        public ExecuteBots(Program program) : base("Вы уверены что хотите запустить ботов?", program)
        {
            Prog = program;
        }
        public override void Display()
        {
            base.Display();
            VerifyBots();
        }
        public static void VerifyBots()
        {
            Output.WriteLine("Подождите...");
            Task.Run(() =>
            {
                GenerateBots();
            }).Wait();
            var table = new ConsoleTable("Количество Ботов", "Примерное ожидания запуска(секунды)", "Дата");
            table.AddRow(ClientManagers.Count(), new TimeSpan(0, 0, 0, (int)(ClientManagers.Count() * GlobalVars.JoinDelay.TotalSeconds), 0).TotalSeconds, DateTime.Now);
            table.Write(Format.MarkDown);
            Output.WriteLine(ConsoleColor.Green, $"Ожидайте запуск...");
            StartBots();
            MainPage.StartedBots = true;
            Output.WriteLine(ConsoleColor.Green, $"Успех.");
            table.AddRow($"Done", 0, 0);
            table.Write(Format.MarkDown);
            Input.ReadString("Нажмите [Enter] Чтобы вернуться на главную страницу");
            Prog.NavigateHome();
        }

        private static void GenerateBots()
        {
            try
            {

                ClientManagers = new List<ClientManager>();
                Dict = new Dictionary<Time, List<Bot>>();
                var Bots = GlobalVars.DataBase.FindAll<ChannelType>().ToList();
                //List<BotsJson> Bots = JsonConvert.DeserializeObject<List<BotsJson>>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Bots.json")));
                for (int i = 0; i < Bots.Count; i++)
                {
                    var bot = Bots.ElementAt(i);
                    Dict.Add(bot.Time, bot.Bots);
                    for (int x = 0; x < bot.Bots.Count; x++)
                    {
                        var token = bot.Bots.ElementAt(x);
                        if (!bot.IsChannel)
                        {
                            if (!GlobalVars.DiscChannels.ContainsKey(bot.ChannelID))
                                FetchCategory(bot.ChannelID, token.Token);
                            var EmptyChannel = GlobalVars.DiscChannels[bot.ChannelID].Where(x => (x.UsersIn < x.UserLimit) || x.UserLimit == 0).FirstOrDefault();
                            if (EmptyChannel != null)
                                if ((EmptyChannel.UsersIn < EmptyChannel.UserLimit) || EmptyChannel.UserLimit == 0)
                                {
                                    ClientManagers.Add(new ClientManager(new Client(token.Token, GlobalVars.GuildID, EmptyChannel.Id, bot.IsChannel, bot.SpecialChannel)));
                                    EmptyChannel.UsersIn++;
                                }

                        }
                        else
                            ClientManagers.Add(new ClientManager(new Client(token.Token, GlobalVars.GuildID, bot.ChannelID, bot.IsChannel, bot.SpecialChannel)));

                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("WriteLines.txt", ex.Message);
                throw ex;
            }

        }
        public static void FetchCategory(string ParentId, string Token)
        {
            string baseURL = $"https://discord.com/api/v7/guilds/{GlobalVars.GuildID}/channels";
            try
            {
                WebRequest request = WebRequest.Create(baseURL);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ContentType = "application/json";
                request.Method = "GET";
                request.Headers.Set("Authorization", Token);
                WebResponse response = request.GetResponse();
                using Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                var Channels = JsonConvert.DeserializeObject<List<Channel>>(responseFromServer);
                var CatChannels = Channels.FindAll(x => x.ParentId == ParentId);
                GlobalVars.DiscChannels.Add(ParentId, CatChannels);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void StartBots()
        {
            Task.Run(async () =>
            {
                try
                {
                    for (int i = 0; i < Dict.Count; i++)
                    {
                        StdSchedulerFactory factory = new StdSchedulerFactory();

                        // get a scheduler
                        IScheduler scheduler = await factory.GetScheduler();
                        await scheduler.Start();

                        KeyValuePair<Time, List<Bot>> dic = Dict.ElementAt(i);
                        var leaveTime = VerifyTime(dic.Key.LeaveTime);
                        var joinTime = VerifyTime(dic.Key.JoinTime);

                        Output.WriteLine(ConsoleColor.Green, $"Channel:{dic.Value.ToArray()}\njoin:{joinTime}\nleave:{leaveTime}");
                        //Join trigger
                        ITrigger joinTrigger = TriggerBuilder.Create()
                                                            .StartAt(joinTime)
                                                            .WithSimpleSchedule(x => x
                                                                .WithIntervalInHours(24)
                                                                .RepeatForever())
                                                            .Build();
                        IJobDetail joinJob = JobBuilder.Create<ExecuteBot>()
                        .Build();
                        joinJob.JobDataMap.Put("JoinBots", dic);
                        await scheduler.ScheduleJob(joinJob, joinTrigger);
                        //Leave Trigger
                        ITrigger leaveTrigger = TriggerBuilder.Create()
                                                            .StartAt(leaveTime)
                                                            .WithSimpleSchedule(x => x
                                                                .WithIntervalInHours(24)
                                                                .RepeatForever())
                                                            .Build();
                        IJobDetail leaveJob = JobBuilder.Create<ExecuteBot>()
                        .Build();
                        leaveJob.JobDataMap.Put("LeaveBots", dic);
                        await scheduler.ScheduleJob(leaveJob, leaveTrigger);

                        //var dif = leaveTime.Ticks - DateTime.Now.Ticks;

                        //if (dif >= TimeSpan.TicksPerHour)
                        //{


                        //    ScheduleAction(async () =>
                        //    {
                        //        for (int i = 0; i < dic.Value.Count; i++)
                        //        {
                        //            var bot = dic.Value.ElementAt(i);
                        //            var client = ClientManagers.Find(x => x.Client.Token == bot.Token);
                        //            await Task.Delay(GlobalVars.JoinDelay);
                        //            client.StartAsync(bot.Token);

                        //        }
                        //    }, joinTime);
                        //    ScheduleAction(async () =>
                        //    {
                        //        for (int i = 0; i < dic.Value.Count; i++)
                        //        {
                        //            var bot = dic.Value.ElementAt(i);
                        //            var client = ClientManagers.Find(x => x.Client.Token == bot.Token);
                        //            await Task.Delay(GlobalVars.JoinDelay);
                        //            client.Disconnect(bot.Token);
                        //        }
                        //    }, leaveTime);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

        }

        private static DateTime VerifyTime(DateTime dateTime)
        {
            var newDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            //newDate = TimeZoneInfo.ConvertTime(newDate, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
            return newDate <= DateTime.Now ? newDate.AddDays(1) : newDate;
        }
    }
}
