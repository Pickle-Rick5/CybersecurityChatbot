using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CybersecurityChatbotPart2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Reuses the same WinMM voice greeting from Part 1
            CybersecurityChatbot.Visuals.PlayVoiceGreeting("welcome.wav");

            AddMessage("Chatbot: Welcome! GUI shell is up and running. Chat logic arrives in the next commit.", "#00FF9C");
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendCurrentInput();
        }

        private void InputTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendCurrentInput();
            }
        }

        private void SendCurrentInput()
        {
            string text = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            AddMessage($"You: {text}", "#FFD966");
            InputTextBox.Clear();

            // Placeholder — real ChatEngine wiring happens in Commit 2
            AddMessage("Chatbot: (logic not connected yet)", "#00FF9C");
        }

        private void AddMessage(string text, string hexColor)
        {
            var block = new TextBlock
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