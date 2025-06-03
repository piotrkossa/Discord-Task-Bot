using System;
using System.IO;

namespace DiscordTaskBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Env.LoadEnv(".env");

            var bot = new Bot();
            await bot.RunAsync();
        }
    }
}