using System;

namespace CybersecurityChatbot
{
    public class ChatEngine
    {
        private readonly string _userName;

        public ChatEngine(string userName)
        {
            _userName = userName;
        }

        // Question 4 & 5: Processing Input & Generating Responses
        public string GetResponse(string rawInput, out bool shouldExit)
        {
            shouldExit = false;
            string cleanInput = rawInput.Trim().ToLower();

            // Exit handling
            if (cleanInput == "exit" || cleanInput == "quit")
            {
                shouldExit = true;
                return $"Goodbye, {_userName}! Remember to stay alert and secure online.";
            }

            // General queries
            if (cleanInput.Contains("how are you"))
            {
                return "I am operating optimally! Ready to help protect your digital footprint.";
            }
            if (cleanInput.Contains("purpose"))
            {
                return "My purpose is to educate citizens against growing cyber threat landscapes in South Africa.";
            }
            if (cleanInput.Contains("ask you about") || cleanInput == "help")
            {
                Visuals.ShowHelpMenu();
                return "Please choose one of the core security topics displayed above!";
            }

            // Core Cybersecurity Education Content
            if (cleanInput.Contains("password"))
            {
                return "[Password Safety] Combine at least 12 characters using long phrases, numbers, and symbols. Never reuse credentials across accounts.";
            }
            if (cleanInput.Contains("phishing"))
            {
                return "[Phishing Alerts] Be suspicious of unsolicited emails or banking SMS notifications requesting urgent verification links.";
            }
            if (cleanInput.Contains("browsing"))
            {
                return "[Safe Browsing] Always verify URLs look authentic, confirm the HTTPS padlock icon is active, and skip public Wi-Fi transactions.";
            }

            // Question 5: Graceful Input Validation Fallback
            return "I didn't quite understand that. Could you try rephrasing your question or checking your spelling?";
        }
    }
}