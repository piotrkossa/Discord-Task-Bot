using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;

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
            return new TaskData(description, user.Id, DateTime.Today, DateTime.Today.AddDays(daysToDeadline), TaskStates.NOT_STARTED, channelID);
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
            LoadTasks();
        }

        public static void DeleteNonExistentTasks()
        {
            
        }
    }
}