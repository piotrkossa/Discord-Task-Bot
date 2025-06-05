using Discord;
using Discord.Interactions;
using DiscordTaskBot.Misc;

namespace DiscordTaskBot.Modules
{
    public class TaskSlashModules : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("createtask", "Creates new task")]
        public async Task CreateTask(
            [Summary("description", "Description of the task")] string description,
            [Summary("user", "User to whom the task will be assigned")] IUser user,
            [Summary("daysToDeadline", "Days allocated to complete the task")] int daysToDeadline)
        {
            await DeferAsync();

            var taskID = Guid.NewGuid().ToString();

            var taskData = TaskData.FromDiscord(description, user, daysToDeadline, Context.Channel.Id);

            var message = await MessageLogic.SendTaskMessageAsync(taskID, taskData, Context);

            taskData.MessageID = message.Id;

            TaskManager.AddTask(taskID, taskData);
        }
    }
}