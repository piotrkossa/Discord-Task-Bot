using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using System.Reflection;
using DiscordTaskBot.Misc;

namespace DiscordTaskBot
{
    public class Bot
    {
        public static readonly DiscordSocketClient _client = new(
            new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged
            });

        private readonly InteractionService _interactionService;

        public Bot()
        {
            _interactionService = new InteractionService(_client.Rest);

            // Client Actions
            _client.Ready += OnReady;
            _client.Log += LogAsync;
            _client.InteractionCreated += OnInteraction;
            _client.ButtonExecuted += TaskStatusHandler.ButtonHandler;
        }

        public async Task RunAsync()
        {
            // Checking if Discord Bot Token is Specified
            if (!CheckEnviromentalVariables())
            {
                return;
            }

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("TOKEN"));
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task dailyTaskUpdater;

        private async Task OnReady()
        {
            Console.WriteLine("Bot connected.");

            await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), null);

            await _interactionService.RegisterCommandsToGuildAsync(ulong.Parse(Environment.GetEnvironmentVariable("GUILD")!), true);

            Console.WriteLine("Slash commands registered.");

            TaskManager.LoadTasks();
            await TaskManager.DeleteInactiveTasks();
            Console.WriteLine("Tasks loaded.");

            if (dailyTaskUpdater == null)
                dailyTaskUpdater = ScheduleDailyUpdateAsync();

            Console.WriteLine("Updating every 24h enabled.");
        }

        private async Task OnInteraction(SocketInteraction interaction)
        {
            var context = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(context, null);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine($"[LOG] {log}");
            return Task.CompletedTask;
        }

        private bool CheckEnviromentalVariables()
        {
            if (Environment.GetEnvironmentVariable("TOKEN") == null)
            {
                Console.Error.Write("Token Not Specified!");
                return false;
            }
            if (Environment.GetEnvironmentVariable("GUILD") == null || !ulong.TryParse(Environment.GetEnvironmentVariable("GUILD"), out ulong _))
            {
                Console.Error.Write("Guild Not Specified or Incorrect!");
                return false;
            }
            if (Environment.GetEnvironmentVariable("ARCHIVE_CHANNEL") == null || !ulong.TryParse(Environment.GetEnvironmentVariable("GUILD"), out ulong _))
            {
                Console.Error.Write("Archive Channel ID Not Specified or Incorrect!");
                return false;
            }

            return true;
        }

        private static async Task ScheduleDailyUpdateAsync()
        {
            while (true)
            {
                DateTime now = DateTime.Now;
                DateTime nextRun = now.Date.AddDays(1).AddMinutes(1);

                TimeSpan delay = nextRun - now;

                await Task.Delay(delay);

                TaskManager.UpdateArchivisedTasks();
                await MessageLogic.DailyTaskUpdate();
                Console.WriteLine("Tasks updated!");
            }
        }
    }
}