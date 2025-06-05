using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordTaskBot.Misc
{
    public static class MessageLogic
    {
        public static async Task<IUserMessage> SendTaskMessageAsync(string taskID, TaskData taskData, SocketInteractionContext context)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Task")
                .WithDescription($"{taskData.Description}")
                .AddField("Assigned To", $"<@{taskData.UserID}>", inline: true)  // mention the user
                .AddField("Deadline", taskData.CompletionDate.ToString("MM/dd/yyyy"), inline: true)  // nicer date format
                .AddField("Status", taskData.State.ToString(), inline: true)
                .WithColor(Color.Orange)
                .WithFooter(footer => footer.Text = $"Created on: {taskData.CreationDate:dd/MM/yyyy}")
                .Build();

            var component = new ComponentBuilder()
                .WithButton("In Progress", customId: $"state_{taskID}", ButtonStyle.Primary)
                .WithButton("Cancel", customId: $"delete_{taskID}", ButtonStyle.Danger)
                .Build();


            await context.Interaction.FollowupAsync(embed: embed, components: component);

            return await context.Interaction.GetOriginalResponseAsync();
        }
    }
}