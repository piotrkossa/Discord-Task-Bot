using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;
using Discord.WebSocket;

namespace DiscordTaskBot.Misc
{
    public enum TaskStates
    {
        NOT_STARTED,
        IN_PROGRESS,
        COMPLETE,
        ARCHIVE
    }

    public class TaskData
    {
        public string Description { get; set; }
        public ulong UserID { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime CompletionDate { get; set; }

        public TaskStates State { get; set; }

        public ulong ChannelID { get; set; }
        public ulong MessageID { get; set; }

        public static TaskData FromDiscord(string description, IUser user, int daysToDeadline, ulong channelID)
        {
            return new TaskData(description, user.Id, DateTime.Today, DateTime.Today.AddDays(daysToDeadline+1).AddSeconds(-1), TaskStates.NOT_STARTED, channelID);
        }

        [JsonConstructor]
        public TaskData(string description, ulong userID, DateTime creationDate, DateTime completionDate, TaskStates state, ulong channelID, ulong messageID = 0)
        {
            Description = description;
            UserID = userID;
            CreationDate = creationDate;
            CompletionDate = completionDate;
            State = state;
            ChannelID = channelID;
            MessageID = messageID;
        }
    }

    public static class TaskManager
    {
        private const string FilePath = "tasks.json";

        public static Dictionary<string, TaskData> Tasks { get; private set; } = [];

        public static void LoadTasks()
        {
            if (!File.Exists(FilePath))
            {
                Tasks = [];
                return;
            }

            string json = File.ReadAllText(FilePath);
            Tasks = JsonSerializer.Deserialize<Dictionary<string, TaskData>>(json) ?? [];
        }

        public static void SaveTasks()
        {
            string json = JsonSerializer.Serialize(Tasks, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(FilePath, json);
        }

        public static void AddTask(string taskID, TaskData task)
        {
            Tasks.Add(taskID, task);
            SaveTasks();
        }

        public static void RemoveTask(string taskID)
        {
            if (Tasks.Remove(taskID)) SaveTasks();
        }


        public static async Task<IUserMessage?> GetUserMessageById(string taskID)
        {
            if (!Tasks.ContainsKey(taskID))
                return null;

            var channel = await Bot._client.GetChannelAsync(Tasks[taskID].ChannelID) as ISocketMessageChannel;
            if (channel == null)
                return null;

            return await channel.GetMessageAsync(Tasks[taskID].MessageID) as IUserMessage;
        }

        public static void UpperTaskState(string taskID)
        {
            if (!Tasks.ContainsKey(taskID))
                return;

            if (Tasks[taskID].State < Enum.GetValues<TaskStates>().Max())
            {
                Tasks[taskID].State += 1;
                SaveTasks();
            }
        }

        public static async Task DeleteInactiveTasks()
        {
            var keys = Tasks.Keys;
            foreach (var taskID in keys)
            {
                if (await GetUserMessageById(taskID) == null)
                {
                    Tasks.Remove(taskID);
                }
            }

            SaveTasks();
        }
    }
}