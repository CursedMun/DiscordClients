using EasyConsole;

using Newtonsoft.Json;

using Serilog;

using System;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Websocket.Client;
namespace DiscordClients.Helpers
{
    public class ClientManager
    {
        public ManualResetEvent exitEvent = new ManualResetEvent(false);
        public readonly IWebsocketClient _client;
        public Client Client;
        public ClientManager(Client client)
        {
            Client = client;
            _client = new WebsocketClient(new Uri("wss://gateway.discord.gg/?v=8&encoding=json"), new Func<ClientWebSocket>(() =>
            {
                var client = new ClientWebSocket
                {
                    Options =
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(120),
                    }
                };
                return client;
            }))
            {
                Name = "Discord",
                ReconnectTimeout = null,
                ErrorReconnectTimeout = TimeSpan.FromSeconds(30),
            };
        }

        public void StartAsync(string id)
        {
            if (Client.Special) return;
            _client.ReconnectionHappened.Subscribe(type =>
                {
                    StartClient();
                    ConnectClient();
                    //if (!Client.Alive)
                    //{

                    //}
                });
            _client.DisconnectionHappened.Subscribe(info =>
               {
                   //Client.Alive = false;
                   //Client.Restart = true;
                   //if ((int)info.CloseStatus.Value == 4004)
                   //{
                   //    Client.Restart = false;
                   //    Client.Alive = true;
                   //    exitEvent.Set();
                   //    System.Console.WriteLine("Client Removed: " + Client.Token);
                   //}

               });
            _client.MessageReceived.Subscribe(async msg =>
                {
                    if (msg.Text.Contains("\"op\":10"))
                    {
                        var discordEvent = JsonConvert.DeserializeObject<DiscordEvent>(msg.Text);
                        Heartbeat((int)discordEvent.D.heartbeat_interval);
                    }
                    else if (msg.Text.Contains("\"t\":\"READY\""))
                    {
                        var discordEvent = JsonConvert.DeserializeObject<DiscordEvent>(msg.Text);
                        Client.UserID = discordEvent.D.User.Id;
                    }
                    else if (msg.Text.Contains("\"t\":\"VOICE_STATE_UPDATE\""))
                    {
                        var discordEvent = JsonConvert.DeserializeObject<DiscordEvent>(msg.Text);
                        if (discordEvent.D.UserId == Client.UserID && discordEvent.D.ChannelId == null && Client.Connected)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(30));
                            ConnectClient();
                        }
                        else if (discordEvent.D.UserId == Client.UserID)
                        {
                        }
                    }
                });
            _client.Start().Wait();
            Output.WriteLine($"запустил {id}");
            StartClient();
            ConnectClient();
            exitEvent.WaitOne();
        }
        public void Disconnect(string id)
        {
            DisconnectClient();
            Stop();
            Output.WriteLine($"Disconnected {id}");
        }
        public void Heartbeat(int ms)
        {
            var Interval = new Interval();
            Interval.Set(() =>
            {
                _client.Send(JsonConvert.SerializeObject(new { op = 1, d = (object)null }));
                if (Client.Alive)
                    Output.WriteLine($"HeartBeated {Client.Token}");
                else
                {
                    //Interval.Dispose();
                    Output.WriteLine($"Stopped the heartbeat for this user {Client.UserID} | {Client.Token}");

                }
            }, ms);
        }
        public async Task SetInterval(Action action, TimeSpan timeout)
        {
            await Task.Delay(timeout).ConfigureAwait(false);
            action();
            await SetInterval(action, timeout);

        }
        private void StartClient()
        {
            var msg = new
            {
                token = Client.Token,
                properties = new
                {
                    os = "linux",
                    browser = "disco",
                    device = "disco"
                },
                compress = false,
                presence = Client.Presence
            };
            var payload = new
            {
                op = 2,
                d = msg
            };

            _client.Send(JsonConvert.SerializeObject(payload));

        }

        private void ConnectClient()
        {
            var msg = new
            {
                op = 4,
                d = new
                {
                    guild_id = GlobalVars.GuildID,
                    channel_id = Client.ChannelID,
                    self_mute = Client.Micro,
                    self_deaf = Client.Sound
                },
            };

            _client.Send(JsonConvert.SerializeObject(msg));
            Client.Connected = true;
            Output.WriteLine(ConsoleColor.Green, $"Зашёл в канал {Client.ChannelID}");
        }

        private void DisconnectClient()
        {
            if (!Client.Connected) Output.WriteLine("Error the client isnt connected to any instance");
            Client.Connected = false;
            var msg = new
            {
                op = 4,
                d = new
                {
                    guild_id = "null",
                    channel_id = "null",
                    self_mute = "null",
                    self_deaf = "null"
                },
            };

            _client.Send(JsonConvert.SerializeObject(msg));
        }
        public void Stop()
        {
            // Logging
            _client.IsReconnectionEnabled = false;
            _client.Dispose();
            //await _client.Stop(WebSocketCloseStatus.Empty, string.Empty);
            Client.Connected = false;
            Client.Alive = false;
            Log.Logger.Error($"Closed connexion to WebSocket ");

        }
        private static void InitLogging()
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var logPath = Path.Combine(executingDir, "logs", "verbose.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
