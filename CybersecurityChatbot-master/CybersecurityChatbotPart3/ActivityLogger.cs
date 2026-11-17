using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbotPart3
{
    // Keeps a running record of the actions the chatbot takes for the user.
    public class ActivityLogger
    {
        private const int RecentCount = 8; // the brief asks for only the last 5-10 actions

        private readonly List<LogEntry> _entries = new List<LogEntry>();

        private record LogEntry(DateTime Time, string Description);

        public int Count => _entries.Count;
        public bool HasMoreThanRecent => _entries.Count > RecentCount;

        public void Add(string description)
        {
            _entries.Add(new LogEntry(DateTime.Now, description));
        }

        public string GetSummary(bool showAll = false)
        {
            if (_entries.Count == 0)
                return "I haven't taken any actions yet. Try adding a task, setting a reminder or starting the quiz!";

            int skip = showAll ? 0 : Math.Max(0, _entries.Count - RecentCount);
            var shown = _entries.Skip(skip).ToList();

            string header = showAll
                ? "Here's the full history of my actions:"
                : "Here's a summary of recent actions:";

            var lines = shown.Select((entry, i) => $"{i + 1}. {entry.Description} ({entry.Time:HH:mm})");
            string text = header + "\n" + string.Join("\n", lines);

            if (!showAll && HasMoreThanRecent)
                text += $"\nShowing the last {RecentCount} of {_entries.Count} actions. Type 'show more' to see the full history.";

            return text;
        }
    }
}