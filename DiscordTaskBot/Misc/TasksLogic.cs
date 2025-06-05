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

        //public ulong ChannelID { get; set; } = message.Channel.Id;
        //public ulong MessageID { get; set; } = message.Id;

        public static TaskData FromDiscord(string description, IUser user, int daysToDeadline)
        {
            return new TaskData(description, user.Id, DateTime.Today, DateTime.Today.AddDays(daysToDeadline), TaskStates.NOT_STARTED);
        }

        [JsonConstructor]
        public TaskData(string description, ulong userID, DateTime creationDate, DateTime completionDate, TaskStates state)
        {
            Description = description;
            UserID = userID;
            CreationDate = creationDate;
            CompletionDate = completionDate;
            State = state;
        }
    }

    public static class TaskManager
    {
        private const string FilePath = "tasks.json";

        public static List<TaskData> Tasks { get; private set; } = [];

        public static void LoadTasks()
        {
            if (!File.Exists(FilePath))
            {
                return;
            }

            string json = File.ReadAllText(FilePath);
            Tasks = JsonSerializer.Deserialize<List<TaskData>>(json) ?? [];
        }

        public static void SaveTasks()
        {
            string json = JsonSerializer.Serialize(Tasks, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(FilePath, json);
        }

        public static void AddTask(TaskData task)
        {
            Tasks.Add(task);
            SaveTasks();
            LoadTasks();
        }
    }
}