using System;
using System.Timers;

namespace DiscordClients.Helpers
{
    public class Interval : IDisposable
    {
        private readonly Timer Timer;
        public Interval()
        {
            Timer = new Timer();
        }
        public Timer Set(Action action, int interval)
        {
            Timer.Interval = interval;
            Timer.Elapsed += (s, e) =>
            {
                Timer.Enabled = false;
                action();
                Timer.Enabled = true;
            };
            Timer.Enabled = true;
            return Timer;
        }

        public void Dispose()
        {
            Timer.Stop();
            Timer.Dispose();
        }
    }
}
