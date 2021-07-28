using System;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;

using EasyConsole;

using Newtonsoft.Json;

using Serilog;

using Websocket.Client;
namespace DiscordClients.Helpers
{
    public class ClientManager
    {

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
                    if (!Client.Connected)
                    {
                        StartClient();
                        ConnectClient();
                    }
                });
            _client.DisconnectionHappened.Subscribe(info =>
               {
                   Client.Alive = false;
                   System.Console.WriteLine(info);
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
                            System.Console.WriteLine(msg.Text);
                        }
                        else if (discordEvent.D.UserId == Client.UserID)
                        {
                            System.Console.WriteLine(msg.Text);
                        }
                    }
                });
            _client.Start().Wait();
            Log.Information($"Started {id}");
            StartClient();
            ConnectClient();
        }
        public async void Disconnect(string id)
        {
            DisconnectClient();
            await Stop();
            Log.Information($"Disconnected {id}");
        }
        public void Heartbeat(int ms)
        {
            var Interval = new Interval();
            Interval.Set(() =>
            {
                _client.Send(JsonConvert.SerializeObject(new { op = 1, d = (object)null }));
                if (Client.Alive)
                    Log.Information($"HeartBeated {Client.Token}");
                else
                    Log.Information($"Stopped the heartbeat for this user {Client.UserID} | {Client.Token}");
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
            Output.WriteLine(ConsoleColor.Green, $"захожу в канал {Client.ChannelID}");
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
        }

        private void DisconnectClient()
        {
            if (!Client.Connected) Log.Information("Error the client isnt connected to any instance");
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
        public async Task Stop()
        {
            // Logging
            _client.IsReconnectionEnabled = false;
            await _client.Stop(WebSocketCloseStatus.Empty, string.Empty);
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
