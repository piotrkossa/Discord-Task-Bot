using Discord;
using Discord.Interactions;
using DiscordTaskBot.Misc;

namespace DiscordTaskBot.Modules
{
    public class TaskCreationModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("createtask", "Creates new task")]
        public async Task CreateTask(
            [Summary("description", "Description of the task")] string description,
            [Summary("user", "User to whom the task will be assigned")] IUser user,
            [Summary("daysToDeadline", "Days allocated to complete the task")] int daysToDeadline)
        {
            TaskManager.AddTask(TaskData.FromDiscord(description, user, daysToDeadline));
        }
    }
}