using System.Collections.Generic;

namespace CybersecurityChatbotPart2
{
    public class ChatEngine
    {
        private readonly string _userName;

        // Dictionary of topic -> list of responses.
        // Lists have one entry for now; Commit 3 adds more and randomises selection.
        private readonly Dictionary<string, List<string>> _topicResponses = new Dictionary<string, List<string>>
        {
            ["password"] = new List<string> { "[Password Safety] Combine at least 12 characters using long phrases, numbers, and symbols. Never reuse credentials across accounts." },
            ["phishing"] = new List<string> { "[Phishing Alerts] Be suspicious of unsolicited emails or banking SMS notifications requesting urgent verification links." },
            ["browsing"] = new List<string> { "[Safe Browsing] Always verify URLs look authentic, confirm the HTTPS padlock icon is active, and skip public Wi-Fi transactions." },
            ["scam"] = new List<string> { "[Scam Awareness] Scammers often disguise themselves as trusted organisations. Never send money or personal details based on unsolicited requests." },
            ["privacy"] = new List<string> { "[Privacy Tips] Review your social media privacy settings regularly and limit how much personal information you share publicly." }
        };

        public ChatEngine(string userName)
        {
            _userName = userName;
        }

        public string GetResponse(string rawInput, out bool shouldExit)
        {
            shouldExit = false;
            string cleanInput = rawInput.Trim().ToLower();

            if (cleanInput == "exit" || cleanInput == "quit")
            {
                shouldExit = true;
                return $"Goodbye, {_userName}! Remember to stay alert and secure online.";
            }

            if (cleanInput.Contains("how are you"))
                return "I am operating optimally! Ready to help protect your digital footprint.";

            if (cleanInput.Contains("purpose"))
                return "My purpose is to educate citizens against growing cyber threat landscapes in South Africa.";

            if (cleanInput.Contains("help"))
                return "You can ask me about: password, phishing, browsing, scam, or privacy.";

            foreach (var topic in _topicResponses.Keys)
            {
                if (cleanInput.Contains(topic))
                {
                    return _topicResponses[topic][0]; // random selection arrives in Commit 3
                }
            }

            return "I didn't quite understand that. Could you try rephrasing your question or checking your spelling?";
        }
    }
}