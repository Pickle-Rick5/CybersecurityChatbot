using System.Collections.Generic;

namespace CybersecurityChatbotPart3.Models
{
    public class QuizQuestion
    {
        public string Text { get; set; } = "";
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; } = "";

        public bool IsTrueFalse => Options.Count == 2;
    }
}