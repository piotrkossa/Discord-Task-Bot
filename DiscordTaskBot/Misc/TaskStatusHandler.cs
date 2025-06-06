using Discord.WebSocket;
using DiscordTaskBot.Misc;

public static class TaskStatusHandler
{
    public static async Task ButtonHandler(SocketMessageComponent component)
    {
        var parts = component.Data.CustomId.Split('_');

        await component.DeferAsync(true);

        if (parts.Length != 2)
        {
            await component.FollowupAsync("Invalid button format.", ephemeral: true);
            return;
        }

        var action = parts[0];
        var taskID = parts[1];

        var message = TaskManager.GetUserMessageById(taskID);
        if (message == null || !TaskManager.Tasks.ContainsKey(taskID))
        {
            await component.FollowupAsync("Task not found.", ephemeral: true);
            return;
        }


        var taskData = TaskManager.Tasks[taskID];

        var user = component.User as SocketGuildUser;

        if (user == null)
        {
            await component.FollowupAsync("Could not indentify user.", ephemeral: true);
            return;
        }

        switch (action)
        {
            case "state":
                if (taskData.State < TaskStates.COMPLETE)
                {
                    if (user.Id != taskData.UserID && !user.GuildPermissions.Administrator)
                    {
                        await component.FollowupAsync("It is not your task!", ephemeral: true);
                        return;
                    }
                    TaskManager.UpperTaskState(taskID);
                    await MessageLogic.UpdateTaskMessageStatus(taskData, taskID, component.Message);
                    await component.FollowupAsync("Task status updated!", ephemeral: true);
                }
                else if (taskData.State == TaskStates.COMPLETE)
                {
                    if (!user.GuildPermissions.Administrator)
                    {
                        await component.FollowupAsync("You do not have permissions!", ephemeral: true);
                        return;
                    }
                    TaskManager.UpperTaskState(taskID);
                    await MessageLogic.MoveTaskMessageToArchive(taskData, taskID, component);
                    await component.FollowupAsync("Task moved to archive,", ephemeral: true);
                }
                break;
            case "delete":
                if (!user.GuildPermissions.Administrator)
                {
                    await component.FollowupAsync("You do not have permissions!", ephemeral: true);
                    return;
                }
                TaskManager.RemoveTask(taskID);
                await component.Message.DeleteAsync();
                await component.FollowupAsync("Task deleted.", ephemeral: true);
                return;
        }
    }
}