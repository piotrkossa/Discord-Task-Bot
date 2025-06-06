using Discord.WebSocket;
using DiscordTaskBot.Misc;

public static class TaskStatusHandler
{
    public static async Task ButtonHandler(SocketMessageComponent component)
    {
        var parts = component.Data.CustomId.Split('_');

        if (parts.Length != 2)
        {
            await component.RespondAsync("Invalid button format.", ephemeral: true);
            return;
        }

        var action = parts[0];
        var taskID = parts[1];

        var message = TaskManager.GetUserMessageById(taskID);
        if (message == null || !TaskManager.Tasks.ContainsKey(taskID))
        {
            await component.RespondAsync("Task not found.", ephemeral: true);
            return;
        }


        var taskData = TaskManager.Tasks[taskID];

        var user = component.User as SocketGuildUser;

        if (user == null)
        {
            await component.RespondAsync("Could not indentify user.", ephemeral: true);
            return;
        }

        switch (action)
        {
            case "state":
                if (taskData.State < TaskStates.COMPLETE)
                {
                    if (user.Id != taskData.UserID && !user.GuildPermissions.Administrator)
                    {
                        await component.RespondAsync("It is not your task!", ephemeral: true);
                        return;
                    }
                    TaskManager.UpperTaskState(taskID);
                    await MessageLogic.UpdateTaskMessageStatus(taskData, taskID, component.Message);
                }
                else if (taskData.State == TaskStates.COMPLETE)
                {
                    if (!user.GuildPermissions.Administrator)
                    {
                        await component.RespondAsync("You do not have permissions!", ephemeral: true);
                        return;
                    }
                    TaskManager.UpperTaskState(taskID);
                    await MessageLogic.MoveTaskMessageToArchive(taskData, taskID, component);
                }
                break;
            case "delete":
                if (!user.GuildPermissions.Administrator)
                {
                    await component.RespondAsync("You do not have permissions!", ephemeral: true);
                    return;
                }
                TaskManager.RemoveTask(taskID);
                await component.Message.DeleteAsync();
                await component.RespondAsync("Task deleted.", ephemeral: true);
                return;
        }
    }
}