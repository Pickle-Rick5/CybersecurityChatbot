using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CybersecurityChatbotPart3.Data;

namespace CybersecurityChatbotPart3
{
    // Simulates natural language understanding using keyword detection,
    // phrase patterns and a small amount of conversation state.
    public class NlpProcessor
    {
        private enum Intent { None, AddTask, SetReminder, ShowTasks }
        private enum PendingState { None, AwaitingReminderChoice, AwaitingReminderTime }

        private readonly TaskRepository _repo;

        // Conversation state: the bot asked a follow-up question about a task it just saved
        private PendingState _pending = PendingState.None;
        private int _pendingTaskId;
        private string _pendingTitle = "";

        // Intent -> trigger phrases (checked in order)
        private static readonly List<KeyValuePair<Intent, string[]>> IntentKeywords = new()
        {
            new(Intent.ShowTasks, new[] { "show my tasks", "show tasks", "view my tasks", "view tasks",
                                          "list my tasks", "list tasks", "what are my tasks", "my to-do list" }),
            new(Intent.SetReminder, new[] { "remind me", "set a reminder", "set reminder", "add a reminder",
                                            "add reminder", "create a reminder", "create reminder" }),
            new(Intent.AddTask, new[] { "add a task", "add task", "new task", "create a task",
                                        "create task", "add a to-do", "add to my tasks" })
        };

        // Keyword in the task title -> helpful description the bot fills in
        private static readonly List<(string Keyword, string Description)> DescriptionTemplates = new()
        {
            ("two-factor", "Turn on two-factor authentication to add an extra layer of protection to your accounts."),
            ("2fa", "Turn on two-factor authentication to add an extra layer of protection to your accounts."),
            ("authentication", "Turn on two-factor authentication to add an extra layer of protection to your accounts."),
            ("password", "Update and strengthen your passwords to keep your accounts secure."),
            ("privacy", "Review account privacy settings to ensure your data is protected."),
            ("phishing", "Learn to spot phishing emails and report suspicious messages."),
            ("backup", "Back up your important files so you can recover them after an attack."),
            ("update", "Keep your software and devices updated to close known security gaps.")
        };

        // Patterns that pull the task title out of different phrasings
        private static readonly Regex[] TitlePatterns =
        {
            new(@"remind me\s+(?:.*?\s)??(?:to|about|of)\s+(.+)", RegexOptions.Compiled),
            new(@"(?:add|create|set|new)\s+(?:a\s+|an\s+)?(?:reminder|task|to-do|todo)\s*(?:(?:to|for|about|called|named)\b|-|–|:)?\s*(.+)", RegexOptions.Compiled)
        };

        private static readonly Regex RelativeTimeRegex = new(
            @"\bin\s+(\d+|an?|one|two|three|four|five|six|seven|eight|nine|ten)\s+(minute|hour|day|week)s?\b",
            RegexOptions.Compiled);

        private static readonly Regex TomorrowRegex = new(@"\btomorrow\b", RegexOptions.Compiled);
        private static readonly Regex NextWeekRegex = new(@"\bnext week\b", RegexOptions.Compiled);
        private static readonly Regex YesRegex = new(@"^(yes|yeah|yep|yup|sure|ok|okay|please|y)\b", RegexOptions.Compiled);
        private static readonly Regex NoRegex = new(@"^(no|nope|nah|n)\b|^not now", RegexOptions.Compiled);

        private static readonly Dictionary<string, int> NumberWords = new()
        {
            ["a"] = 1,
            ["an"] = 1,
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3,
            ["four"] = 4,
            ["five"] = 5,
            ["six"] = 6,
            ["seven"] = 7,
            ["eight"] = 8,
            ["nine"] = 9,
            ["ten"] = 10
        };

        public NlpProcessor(TaskRepository repo)
        {
            _repo = repo;
        }

        // Returns true if the NLP layer handled the message, false if the normal ChatEngine should reply
        public bool TryHandle(string rawInput, out string response)
        {
            response = "";
            string lower = rawInput.Trim().ToLower();

            try
            {
                // 1. Is the user answering a question we just asked?
                if (_pending != PendingState.None)
                {
                    if (HandlePendingReply(lower, out response))
                        return true;

                    _pending = PendingState.None; // they moved on to something else
                }

                // 2. Work out what the user wants from keywords
                switch (DetectIntent(lower))
                {
                    case Intent.ShowTasks:
                        response = ShowTasks();
                        return true;
                    case Intent.AddTask:
                        response = CreateTask(lower, isReminderRequest: false);
                        return true;
                    case Intent.SetReminder:
                        response = CreateTask(lower, isReminderRequest: true);
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                response = "I couldn't reach the task database right now. Please check that MySQL is running.";
                return true;
            }
        }

        private static Intent DetectIntent(string lower)
        {
            foreach (var pair in IntentKeywords)
            {
                if (pair.Value.Any(keyword => lower.Contains(keyword)))
                    return pair.Key;
            }
            return Intent.None;
        }

        private string CreateTask(string lower, bool isReminderRequest)
        {
            string title = ExtractTitle(lower);
            bool hasTime = TryParseTime(lower, out DateTime when, out string matched);

            if (hasTime)
                title = title.Replace(matched, " ");

            title = Regex.Replace(title, @"\s+", " ").Trim(' ', '.', '!', '?', ',', '-', '–', ':');

            if (string.IsNullOrWhiteSpace(title))
            {
                return "Sure, what should the task be? For example: 'Add a task to enable two-factor authentication'.";
            }

            title = char.ToUpper(title[0]) + title.Substring(1);
            if (title.Length > 100) title = title.Substring(0, 100);

            string description = GetDescription(title.ToLower());
            int id = _repo.AddTask(title, description, hasTime ? when : null);

            if (hasTime)
            {
                return isReminderRequest
                    ? $"Reminder set for '{title}' {matched} ({FormatWhen(when)})."
                    : $"Task added: '{title}'. I'll remind you {matched} ({FormatWhen(when)}).";
            }

            _pendingTaskId = id;
            _pendingTitle = title;

            if (isReminderRequest)
            {
                _pending = PendingState.AwaitingReminderTime;
                return $"I've saved '{title}'. When should I remind you? For example: 'tomorrow' or 'in 3 days'.";
            }

            _pending = PendingState.AwaitingReminderChoice;
            return $"Task added: '{title}' with the description \"{description}\" Would you like to set a reminder for this task?";
        }

        private bool HandlePendingReply(string lower, out string response)
        {
            response = "";

            if (TryParseTime(lower, out DateTime when, out string matched))
            {
                _repo.UpdateReminder(_pendingTaskId, when);
                response = $"Got it! I'll remind you {matched} ({FormatWhen(when)}) about '{_pendingTitle}'.";
                _pending = PendingState.None;
                return true;
            }

            if (YesRegex.IsMatch(lower))
            {
                _pending = PendingState.AwaitingReminderTime;
                response = "When should I remind you? For example: 'tomorrow', 'in 3 days' or 'in 2 hours'.";
                return true;
            }

            if (NoRegex.IsMatch(lower))
            {
                _pending = PendingState.None;
                response = $"No problem. '{_pendingTitle}' is saved without a reminder.";
                return true;
            }

            return false;
        }

        private string ShowTasks()
        {
            var tasks = _repo.GetAllTasks().Where(t => !t.IsCompleted).Take(5).ToList();

            if (tasks.Count == 0)
                return "You have no pending tasks. Try: 'Add a task to enable two-factor authentication'.";

            var lines = tasks.Select((t, i) => $"{i + 1}. {t.Title} ({t.ReminderText})");
            return "Here are your pending tasks:\n" + string.Join("\n", lines) +
                   "\nUse the Tasks button to complete or delete them.";
        }

        private static string ExtractTitle(string lower)
        {
            foreach (var pattern in TitlePatterns)
            {
                Match match = pattern.Match(lower);
                if (match.Success)
                    return match.Groups[1].Value;
            }
            return "";
        }

        private static string GetDescription(string lowerTitle)
        {
            foreach (var (keyword, description) in DescriptionTemplates)
            {
                if (lowerTitle.Contains(keyword))
                    return description;
            }
            return "Cybersecurity task added from the chat.";
        }

        private static bool TryParseTime(string lower, out DateTime when, out string matched)
        {
            when = default;
            matched = "";

            Match relative = RelativeTimeRegex.Match(lower);
            if (relative.Success)
            {
                string amountText = relative.Groups[1].Value;
                int amount;
                if (!int.TryParse(amountText, out amount) && !NumberWords.TryGetValue(amountText, out amount))
                    return false;
                if (amount < 1 || amount > 365)
                    return false;

                DateTime now = DateTime.Now;
                switch (relative.Groups[2].Value)
                {
                    case "minute": when = now.AddMinutes(amount); break;
                    case "hour": when = now.AddHours(amount); break;
                    case "day": when = DateTime.Today.AddDays(amount); break;
                    default: when = DateTime.Today.AddDays(7 * amount); break;
                }
                matched = relative.Value;
                return true;
            }

            Match tomorrow = TomorrowRegex.Match(lower);
            if (tomorrow.Success)
            {
                when = DateTime.Today.AddDays(1);
                matched = tomorrow.Value;
                return true;
            }

            Match nextWeek = NextWeekRegex.Match(lower);
            if (nextWeek.Success)
            {
                when = DateTime.Today.AddDays(7);
                matched = nextWeek.Value;
                return true;
            }

            return false;
        }

        private static string FormatWhen(DateTime when) =>
            when.TimeOfDay == TimeSpan.Zero ? when.ToString("dd MMM yyyy") : when.ToString("dd MMM yyyy HH:mm");
    }
}