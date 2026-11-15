using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CybersecurityChatbotPart3.Data;
using CybersecurityChatbotPart3.Models;

namespace CybersecurityChatbotPart3
{
    public partial class QuizWindow : Window
    {
        private static readonly Brush GreenBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x9C));
        private static readonly Brush RedBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x6B, 0x6B));
        private static readonly Brush GrayBrush = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88));

        private readonly List<QuizQuestion> _questions;
        private int _index;
        private int _score;
        private int _answeredCount;
        private bool _answered;
        private bool _finished;

        // Raised once when the window closes: (questions answered, score, total questions, quiz completed)
        public event Action<int, int, int, bool>? QuizEnded;

        public QuizWindow()
        {
            InitializeComponent();
            _questions = QuizBank.GetShuffledQuestions();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => ShowQuestion();

        private void ShowQuestion()
        {
            QuizQuestion q = _questions[_index];
            _answered = false;

            ProgressTextBlock.Text = $"Question {_index + 1} of {_questions.Count}     Score: {_score}";
            QuestionTextBlock.Text = q.Text;
            FeedbackTextBlock.Text = "";
            OptionsPanel.Children.Clear();

            for (int i = 0; i < q.Options.Count; i++)
            {
                string label = q.IsTrueFalse ? q.Options[i] : $"{(char)('A' + i)}) {q.Options[i]}";
                var radio = new RadioButton
                {
                    GroupName = "Answer",
                    Tag = i,
                    Content = new TextBlock { Text = label, TextWrapping = TextWrapping.Wrap }
                };
                OptionsPanel.Children.Add(radio);
            }

            SubmitButton.IsEnabled = true;
            NextButton.IsEnabled = false;
            NextButton.Content = _index == _questions.Count - 1 ? "Finish" : "Next Question";
        }

        private int GetSelectedIndex()
        {
            foreach (var child in OptionsPanel.Children)
            {
                if (child is RadioButton radio && radio.IsChecked == true)
                    return (int)radio.Tag;
            }
            return -1;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_answered) return;

            int selected = GetSelectedIndex();
            if (selected < 0)
            {
                FeedbackTextBlock.Foreground = RedBrush;
                FeedbackTextBlock.Text = "Please select an answer first.";
                return;
            }

            _answered = true;
            _answeredCount++;

            QuizQuestion q = _questions[_index];
            bool correct = selected == q.CorrectIndex;

            if (correct)
            {
                _score++;
                FeedbackTextBlock.Foreground = GreenBrush;
                FeedbackTextBlock.Text = "Correct! " + q.Explanation;
            }
            else
            {
                string correctLabel = q.IsTrueFalse
                    ? q.Options[q.CorrectIndex]
                    : $"{(char)('A' + q.CorrectIndex)}) {q.Options[q.CorrectIndex]}";
                FeedbackTextBlock.Foreground = RedBrush;
                FeedbackTextBlock.Text = $"Not quite. The correct answer is {correctLabel}.\n\n{q.Explanation}";
            }

            // Colour the options: correct one green, a wrong pick red, the rest grey
            foreach (var child in OptionsPanel.Children)
            {
                if (child is RadioButton radio && radio.Content is TextBlock text)
                {
                    int optionIndex = (int)radio.Tag;
                    if (optionIndex == q.CorrectIndex)
                        text.Foreground = GreenBrush;
                    else if (optionIndex == selected)
                        text.Foreground = RedBrush;
                    else
                        text.Foreground = GrayBrush;

                    radio.IsEnabled = false;
                }
            }

            ProgressTextBlock.Text = $"Question {_index + 1} of {_questions.Count}     Score: {_score}";
            SubmitButton.IsEnabled = false;
            NextButton.IsEnabled = true;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_finished)
            {
                Close();
                return;
            }

            if (_index < _questions.Count - 1)
            {
                _index++;
                ShowQuestion();
            }
            else
            {
                ShowResults();
            }
        }

        private void ShowResults()
        {
            _finished = true;

            ProgressTextBlock.Text = "Quiz complete!";
            QuestionTextBlock.Text = $"You scored {_score} out of {_questions.Count}.";
            OptionsPanel.Children.Clear();

            FeedbackTextBlock.Foreground = GreenBrush;
            FeedbackTextBlock.Text = GetFinalFeedback(_score, _questions.Count);

            SubmitButton.IsEnabled = false;
            NextButton.Content = "Close";
            NextButton.IsEnabled = true;
        }

        public static string GetFinalFeedback(int score, int total)
        {
            double percent = total == 0 ? 0 : (double)score / total * 100;

            if (percent >= 80)
                return "Great job! You're a cybersecurity pro!";
            if (percent >= 50)
                return "Good effort! You know the basics. Review the tips and try again to improve your score.";
            return "Keep learning to stay safe online!";
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            bool completed = _finished || _answeredCount == _questions.Count;
            QuizEnded?.Invoke(_answeredCount, _score, _questions.Count, completed);
        }
    }
}