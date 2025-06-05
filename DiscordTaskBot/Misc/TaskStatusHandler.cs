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

        switch (action)
        {
            case "state":
                if (taskData.State != TaskStates.ARCHIVE)
                {
                    TaskManager.UpperTaskState(taskID);
                    await MessageLogic.UpdateTaskMessageStatus(taskData, taskID, component);
                }
                else
                {
                    await MessageLogic.MoveTaskMessageToArchive(taskData, taskID, component);
                }
                break;
            case "delete":
                TaskManager.RemoveTask(taskID);
                await component.Message.DeleteAsync();
                await component.RespondAsync("Task deleted.", ephemeral: true);
                return;
        }
    }
}