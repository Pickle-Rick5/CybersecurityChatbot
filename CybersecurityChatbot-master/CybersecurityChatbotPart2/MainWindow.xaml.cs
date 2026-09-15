using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Media;

namespace CybersecurityChatbotPart2
{
    public partial class MainWindow : Window
    {
        private ChatEngine _engine;
        private bool _awaitingName = true;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PlayGreeting();
            AddMessage("Chatbot: Hello! Please enter your name to start our safety briefing:", "#00FF9C");
        }

        private void PlayGreeting()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "welcome.wav");
                System.Diagnostics.Debug.WriteLine($"Looking for audio at: {path}");
                System.Diagnostics.Debug.WriteLine($"File exists: {File.Exists(path)}");

                if (File.Exists(path))
                {
                    var player = new SoundPlayer(path);
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio error: {ex.Message}");
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => SendCurrentInput();

        private void InputTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                SendCurrentInput();
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
                _awaitingName = false;
                AddMessage($"Chatbot: Welcome, {text}! Ask me about password, phishing, browsing, scam, or privacy.", "#00FF9C");
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