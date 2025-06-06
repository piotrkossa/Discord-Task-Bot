using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordTaskBot.Misc
{
    public static class MessageLogic
    {
        public static async Task<IUserMessage> SendTaskMessageAsync(string taskID, TaskData taskData, SocketInteractionContext context)
        {
            var (embed, component) = BuildMessage(taskData, taskID);


            await context.Interaction.FollowupAsync(embed: embed, components: component);

            return await context.Interaction.GetOriginalResponseAsync();
        }

        public static async Task UpdateTaskMessageStatus(TaskData taskData, string taskID, SocketMessageComponent messageComponent)
        {
            var (embed, component) = BuildMessage(taskData, taskID);

            await messageComponent.UpdateAsync(msg =>
            {
                msg.Embed = embed;
                msg.Components = component;
            });
        }

        public static async Task MoveTaskMessageToArchive(TaskData taskData, string taskID, SocketMessageComponent messageComponent)
        {
            var (embed, component) = BuildMessage(taskData, taskID);

            var oldMessage = await messageComponent.Channel.GetMessageAsync(taskData.MessageID) as IUserMessage;

            var archiveChannelID = ulong.Parse(Environment.GetEnvironmentVariable("ARCHIVE_CHANNEL")!);

            if (oldMessage != null)
                await oldMessage.DeleteAsync();

            var archiveChannel = Bot._client.GetChannel(archiveChannelID) as IMessageChannel;
            if (archiveChannel == null)
                throw new InvalidOperationException("Archive channel not found");

            var newMessage = await archiveChannel.SendMessageAsync(embed: embed, components: component);

            TaskManager.Tasks[taskID].ChannelID = archiveChannelID;
            TaskManager.Tasks[taskID].ChannelID = newMessage.Id;
            TaskManager.SaveTasks();
        }

        private static (Embed, MessageComponent?) BuildMessage(TaskData taskData, string taskID)
        {
            string stateName = "";
            string buttonName = "";
            Color embedColor = Color.Default;

            switch (taskData.State)
            {
                case TaskStates.NOT_STARTED:
                    stateName = "⏳ Not Started";
                    buttonName = "▶️ Start";
                    embedColor = Color.LightGrey;
                    break;
                case TaskStates.IN_PROGRESS:
                    stateName = "🔨 In Progress";
                    buttonName = "🏁 Complete";
                    embedColor = Color.Orange;
                    break;
                case TaskStates.COMPLETE:
                    stateName = "✅ Completed";
                    buttonName = "📥 Archive";
                    embedColor = Color.Green;
                    break;
                case TaskStates.ARCHIVE:
                    stateName = "📦 Archived";
                    embedColor = Color.Purple;
                    break;
            }

            var embed = new EmbedBuilder()
                .WithTitle("Task")
                .WithDescription($"{taskData.Description}")
                .AddField("Assigned To", $"<@{taskData.UserID}>", inline: true)  // mention the user
                //.AddField("Deadline", taskData.CompletionDate.ToString("MM/dd/yyyy"), inline: true)  // nicer date format
                .AddField("Deadline", GetDiscordTimestamp(taskData.CompletionDate), inline: true)  // discord timestamp
                .AddField("Status", stateName, inline: true)
                .WithColor(embedColor)
                .WithFooter(footer => footer.Text = $"Created on: {taskData.CreationDate:dd/MM/yyyy}")
                .Build();


            if (taskData.State == TaskStates.ARCHIVE)
            {
                return (embed, null);
            }

            var component = new ComponentBuilder()
                .WithButton(buttonName, customId: $"state_{taskID}", ButtonStyle.Primary)
                .WithButton("Cancel", customId: $"delete_{taskID}", ButtonStyle.Danger)
                .Build();

            return (embed, component);
        }

        public static string GetDiscordTimestamp(DateTime dateTime, char format = 'R')
        {
            // Upewnij się, że czas jest w UTC
            long unixTime = ((DateTimeOffset)dateTime.ToUniversalTime()).ToUnixTimeSeconds();
            return $"<t:{unixTime}:{format}>";
        }
    }
}