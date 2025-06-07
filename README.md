# Discord Task Bot 📝

A Discord bot for managing tasks on a server – supports statuses, reminders, archiving, and interactive buttons. It stores data in `.json` files, reads from `.env`, and allows editing tasks directly via embeds.

## ✨ Features

- Add tasks with deadlines and descriptions
- Change task status (⚪ Todo → 🟡 In Progress → ✅ Complete → 📦 Archived)
- Automatic embed coloring and archiving
- Interactive buttons for status control (Start, Complete, Archive, Delete)
- Daily updates at 00:01 to refresh task info
- Deadline countdowns with `<t:...:R>` formatting
- Supports multiple users and roles
- Active tasks stored in `tasks.json` and archived in `tasksarchive.json`

## 🛠️ Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- A `.env` file with:

```env
TOKEN=your-bot-token
GUILD=your-server-id
ARCHIVE_CHANNEL=your-archive-task-channel
```

## 🚀 Getting Started

1. Clone the repository:
 ```bash
 git clone https://github.com/YourRepo/DiscordTaskBot.git
 cd DiscordTaskBot
```
2. Create a .env file and set variables.

3. Build and run the bot:
```
dotnet build
dotnet run
```

## ✅ Task Statuses
| Status      | Emoji | Embed Color |
| ----------- | ----- | ----------- |
| Todo        | ⚪    | Gray      |
| In Progress | 🟡    | Orange      |
| Complete    | ✅     | Green       |
| Archived    | 📦    | Purple   |

## 📋 Example Task Message
![Alt text](https://i.imgur.com/5LUW1RO.png)

## 📝 License
Open source – feel free to use, modify, and contribute!
License: MIT
