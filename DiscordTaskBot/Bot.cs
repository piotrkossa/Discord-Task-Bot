using Discord;
using Discord.WebSocket;

namespace DiscordTaskBot
{
    public class Bot
    {
        private DiscordSocketClient _client = new();

        public async Task RunAsync() {
            // Checking if Discord Bot Token is Specified
            if (Environment.GetEnvironmentVariable("TOKEN") == null) {
                Console.Error.Write("Token Not Specified!");
                return;
            }

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("TOKEN"));
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}