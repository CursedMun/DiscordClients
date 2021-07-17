namespace DiscordClients.Helpers
{
    public class Creds
    {
        public class Credentials
        {
            public Agent Agent { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }

            public string Token { get; set; }
        }

        public class Agent
        {
            public string Name { get; set; }

            public int Version { get; set; }
        }

    }
}
