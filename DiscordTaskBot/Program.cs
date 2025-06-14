﻿using DiscordTaskBot.Misc;

namespace DiscordTaskBot
{
    class Program {
        static async Task Main(string[] args) {
            //Loading Enviromental Variables
            EnvLoader.LoadEnv(Path.Combine(AppContext.BaseDirectory, ".env"));

            //Starting Bot and Awaiting for End
            var bot = new Bot();
            await bot.RunAsync();
        }
    }
}