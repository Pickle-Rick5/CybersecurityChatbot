using System;
using System.Collections.Generic;

namespace CybersecurityChatbotPart2
{
    public class ChatEngine
    {
        private readonly string _userName;
        private readonly Random _random = new Random();

        private readonly Dictionary<string, List<string>> _topicResponses = new Dictionary<string, List<string>>
        {
            ["password"] = new List<string>
            {
                "[Password Safety] Combine at least 12 characters using long phrases, numbers, and symbols. Never reuse credentials across accounts.",
                "[Password Safety] Consider using a password manager to generate and store strong, unique passwords for every account.",
                "[Password Safety] Enable multi-factor authentication wherever possible — a password alone is not enough these days."
            },
            ["phishing"] = new List<string>
            {
                "[Phishing Alerts] Be suspicious of unsolicited emails or banking SMS notifications requesting urgent verification links.",
                "[Phishing Alerts] Scammers often disguise themselves as trusted organisations. Always verify a sender's email address carefully.",
                "[Phishing Alerts] Never click links in unexpected messages — rather go directly to the official website by typing it yourself."
            },
            ["browsing"] = new List<string>
            {
                "[Safe Browsing] Always verify URLs look authentic, confirm the HTTPS padlock icon is active, and skip public Wi-Fi transactions.",
                "[Safe Browsing] Avoid downloading software from unofficial or pop-up sources — stick to trusted app stores and vendor sites.",
                "[Safe Browsing] Keep your browser and its extensions updated — outdated software is a common entry point for attackers."
            },
            ["scam"] = new List<string>
            {
                "[Scam Awareness] Scammers often disguise themselves as trusted organisations. Never send money or personal details based on unsolicited requests.",
                "[Scam Awareness] If an offer sounds too good to be true — a prize, a refund, an urgent inheritance — it almost certainly is.",
                "[Scam Awareness] Take your time before acting. Scammers rely on urgency and panic to stop you from thinking clearly."
            },
            ["privacy"] = new List<string>
            {
                "[Privacy Tips] Review your social media privacy settings regularly and limit how much personal information you share publicly.",
                "[Privacy Tips] Be cautious about what personal details you share with apps and websites — only provide what's truly necessary.",
                "[Privacy Tips] Regularly check which apps have access to your location, contacts, and camera, and revoke what you don't need."
            }
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
                    return GetRandomResponse(topic);
                }
            }

            return "I didn't quite understand that. Could you try rephrasing your question or checking your spelling?";
        }

        private string GetRandomResponse(string topic)
        {
            var responses = _topicResponses[topic];
            int index = _random.Next(responses.Count);
            return responses[index];
        }
    }
}
