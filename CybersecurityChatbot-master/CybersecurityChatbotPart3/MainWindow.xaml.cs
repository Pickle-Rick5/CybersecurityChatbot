using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CybersecurityChatbotPart3.Data;

namespace CybersecurityChatbotPart3
{
    public partial class MainWindow : Window
    {
        private ChatEngine _engine = null!;
        private bool _awaitingName = true;
        private readonly TaskRepository _taskRepo = new TaskRepository();
        private NlpProcessor _nlp = null!;
        // Checks every 30 seconds whether any task reminder has come due
        private readonly DispatcherTimer _reminderTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        private readonly HashSet<int> _notifiedReminders = new HashSet<int>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            _reminderTimer.Tick += (s, e) => CheckReminders();
            _reminderTimer.Start();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PlayGreeting();
            AddMessage("Chatbot: Hello! Please enter your name to start our safety briefing:", "#00FF9C");

            if (!_taskRepo.TestConnection(out _))
            {
                AddMessage("Chatbot: Note: I couldn't reach the task database, so task features are unavailable. Everything else works normally.", "#FF6B6B");
            }
        }

        private void PlayGreeting()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "welcome.wav");
                if (File.Exists(path))
                {
                    var player = new SoundPlayer(path);
                    player.Play();
                }
            }
            catch
            {
                // fail silently if audio isn't available
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => SendCurrentInput();

        private void InputTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                SendCurrentInput();
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new TaskWindow(_taskRepo) { Owner = this };
            window.TaskChanged += message => AddMessage($"Chatbot: {message}", "#00FF9C");
            window.ShowDialog();
        }

        private void CheckReminders()
        {
            if (_awaitingName) return;

            try
            {
                foreach (var task in _taskRepo.GetDueReminders())
                {
                    // Add() returns false if we've already told the user about this one
                    if (_notifiedReminders.Add(task.Id))
                    {
                        AddMessage($"Chatbot: Reminder: '{task.Title}' is due. {task.Description}", "#FFD966");
                    }
                }
            }
            catch
            {
                // database unavailable - skip this check
            }
        }

        private void SendCurrentInput()
        {
            string text = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                AddMessage("Chatbot: Input cannot be empty. Please try again.", "#FF6B6B");
                return;
            }

            AddMessage($"You: {text}", "#FFD966");
            InputTextBox.Clear();

            if (_awaitingName)
            {
                _engine = new ChatEngine(text);
                _nlp = new NlpProcessor(_taskRepo);
                _awaitingName = false;
                AddMessage($"Chatbot: Welcome, {text}! Ask me about password, phishing, browsing, scam, or privacy, or click Tasks to manage your cybersecurity to-do list.", "#00FF9C");
                CheckReminders();
                return;
            }

            if (_nlp.TryHandle(text, out string nlpReply))
            {
                AddMessage($"Chatbot: {nlpReply}", "#00FF9C");
                return;
            }
            string response = _engine.GetResponse(text, out bool exit);
            AddMessage($"Chatbot: {response}", exit ? "#FF6B6B" : "#00FF9C");

            if (exit)
                InputTextBox.IsEnabled = false;
        }

        private void AddMessage(string text, string hexColor)
        {
            var block = new System.Windows.Controls.TextBlock
            {
                Text = text,
                FontFamily = new FontFamily("Consolas"),
                Foreground = (Brush)new BrushConverter().ConvertFrom(hexColor),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 4)
            };
            ChatLogPanel.Children.Add(block);
            ChatScrollViewer.ScrollToEnd();
        }
    }
}