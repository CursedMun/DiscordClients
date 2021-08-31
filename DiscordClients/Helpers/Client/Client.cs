using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiscordClients.Helpers
{
    public class Client : INotifyPropertyChanged
    {
        public Client(string token, string guild, string channelid, bool IsChannel, bool special)
        {
            Token = token ?? throw new ArgumentNullException(nameof(Token));
            Guild = guild ?? throw new ArgumentNullException(nameof(Guild));
            Alive = true;
            ChannelID = channelid;
            this.Channel = IsChannel;
            Special = special;
        }

        public readonly string Token;

        public string ChannelID;
        public bool Channel;

        public bool Special;

        private bool _alive = false;
        public bool Alive
        {
            get
            {
                return _alive;
            }
            set
            {
                if (_alive != value)
                    _alive = value;
                NotifyPropertyChanged();
            }
        }
        private bool _connected = false;
        public bool Connected
        {
            get
            {
                return _connected;
            }
            set
            {
                if (_connected != value)
                    _connected = value;
                NotifyPropertyChanged();
            }
        }
        private string _userID;
        public string UserID
        {
            get
            {
                return _userID;
            }
            set
            {
                if (_userID != value)
                    _userID = value;
                NotifyPropertyChanged();
            }
        }
        private bool Random = new Random().Next(0, 100) > 75;
        public bool Micro => Random;

        public bool Sound => Random;
        public object Presence
        {
            get
            {
                var msg = new
                {
                    activities = new List<object>()
                    {
                        new
                        {
                            name = GlobalVars.Statuses[new Random().Next(0, GlobalVars.Statuses.Count)],
                            type = 0,
                            timestamps = new
                            {
                                start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            },
                        },
                    },
                    status = "online",
                    afk = false
                };
                return new Random().Next(0, 100) > 70 ? msg : null;
            }
        }
        public string Guild { get; private set; }
        public bool Restart { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
