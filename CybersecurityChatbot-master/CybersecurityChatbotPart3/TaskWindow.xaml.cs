using System;
using System.Windows;
using System.Windows.Controls;
using CybersecurityChatbotPart3.Data;
using CybersecurityChatbotPart3.Models;

namespace CybersecurityChatbotPart3
{
    public partial class TaskWindow : Window
    {
        private readonly TaskRepository _repo;

        // The main window listens to this so the chatbot can respond in the chat
        public event Action<string>? TaskChanged;

        public TaskWindow(TaskRepository repo)
        {
            InitializeComponent();
            _repo = repo;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => RefreshTasks();

        private void ReminderCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomDatePicker == null) return;
            CustomDatePicker.Visibility = ReminderCombo.SelectedIndex == 4
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private DateTime? GetReminderDate()
        {
            switch (ReminderCombo.SelectedIndex)
            {
                case 1: return DateTime.Today.AddDays(1);
                case 2: return DateTime.Today.AddDays(3);
                case 3: return DateTime.Today.AddDays(7);
                case 4: return CustomDatePicker.SelectedDate;
                default: return null;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                StatusTextBlock.Text = "Please enter a task title.";
                return;
            }

            string description = DescriptionTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(description))
                description = "No description provided.";

            DateTime? reminder = GetReminderDate();

            if (ReminderCombo.SelectedIndex == 4 && reminder == null)
            {
                StatusTextBlock.Text = "Please pick a reminder date, or choose 'No reminder'.";
                return;
            }
            if (reminder.HasValue && reminder.Value < DateTime.Today)
            {
                StatusTextBlock.Text = "The reminder date can't be in the past.";
                return;
            }

            try
            {
                _repo.AddTask(title, description, reminder);
            }
            catch (Exception)
            {
                ShowDbError();
                return;
            }

            string reminderNote = reminder.HasValue
                ? $" I'll remind you on {reminder.Value:dd MMM yyyy}."
                : " (no reminder set)";
            TaskChanged?.Invoke($"Task added: '{title}'.{reminderNote}");

            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            ReminderCombo.SelectedIndex = 0;
            CustomDatePicker.SelectedDate = null;
            RefreshTasks();
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListBox.SelectedItem is not TaskItem task)
            {
                StatusTextBlock.Text = "Select a task in the list first.";
                return;
            }
            if (task.IsCompleted)
            {
                StatusTextBlock.Text = "That task is already completed.";
                return;
            }

            try
            {
                _repo.MarkCompleted(task.Id);
            }
            catch (Exception)
            {
                ShowDbError();
                return;
            }

            TaskChanged?.Invoke($"Nice work! Task completed: '{task.Title}'.");
            RefreshTasks();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListBox.SelectedItem is not TaskItem task)
            {
                StatusTextBlock.Text = "Select a task in the list first.";
                return;
            }

            var answer = MessageBox.Show($"Delete the task '{task.Title}'?", "Confirm delete",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (answer != MessageBoxResult.Yes) return;

            try
            {
                _repo.DeleteTask(task.Id);
            }
            catch (Exception)
            {
                ShowDbError();
                return;
            }

            TaskChanged?.Invoke($"Task deleted: '{task.Title}'.");
            RefreshTasks();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void RefreshTasks()
        {
            try
            {
                TasksListBox.ItemsSource = _repo.GetAllTasks();
                StatusTextBlock.Text = "";
            }
            catch (Exception)
            {
                ShowDbError();
            }
        }

        private void ShowDbError()
        {
            StatusTextBlock.Text = "Couldn't reach the task database. Make sure MySQL is running and dbconfig.txt has your password.";
        }
    }
}