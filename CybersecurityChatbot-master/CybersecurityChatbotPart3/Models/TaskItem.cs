using System;

namespace CybersecurityChatbotPart3.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ReminderText => ReminderDate.HasValue
            ? $"Reminder: {ReminderDate.Value:dd MMM yyyy}"
            : "No reminder";

        public string StatusText => IsCompleted ? "Completed" : "Pending";
    }
}