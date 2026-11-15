using System;
using System.Collections.Generic;
using System.Linq;
using CybersecurityChatbotPart3.Models;

namespace CybersecurityChatbotPart3.Data
{
    public static class QuizBank
    {
        private static readonly Random Rng = new Random();

        // Returns all questions in a fresh random order each time the quiz starts
        public static List<QuizQuestion> GetShuffledQuestions()
        {
            return BuildQuestions().OrderBy(_ => Rng.Next()).ToList();
        }

        private static List<QuizQuestion> BuildQuestions()
        {
            return new List<QuizQuestion>
            {
                MultipleChoice(
                    "What should you do if you receive an email asking for your password?",
                    new[] { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    2,
                    "Reporting phishing emails helps prevent scams and protects other people too."),

                MultipleChoice(
                    "Which of these is the strongest password?",
                    new[] { "password123", "Sam2005", "Tr4in-Coffee-Purple-Moon!", "qwerty" },
                    2,
                    "Long, unpredictable passphrases mixing words, numbers and symbols are hardest to crack. Avoid names, birth years and common words."),

                MultipleChoice(
                    "What does the padlock icon (HTTPS) in your browser actually tell you?",
                    new[] { "The website is guaranteed to be safe", "Your connection to the site is encrypted", "The government has approved the site", "The site is free of viruses" },
                    1,
                    "HTTPS only encrypts the connection. Scam websites can use it too, so always check the address carefully."),

                MultipleChoice(
                    "What is 'social engineering'?",
                    new[] { "Building secure computer networks", "Tricking people into giving up information or access", "A type of antivirus software", "Programming social media apps" },
                    1,
                    "Social engineering targets people rather than technology, using trust, urgency or fear to get information."),

                MultipleChoice(
                    "An SMS from 'your bank' says your account will be locked unless you verify now, with a link. What is the best action?",
                    new[] { "Click the link quickly", "Reply with your details", "Contact your bank using the number on your bank card or official website", "Forward it to your friends" },
                    2,
                    "Urgency is a classic scam tactic. Always use contact details you already trust, never the ones in the message."),

                MultipleChoice(
                    "What is the safest way to use public Wi-Fi?",
                    new[] { "Do your online banking quickly", "Use a VPN and avoid logging in to sensitive accounts", "Turn off all software updates", "Share the Wi-Fi password with others" },
                    1,
                    "Public networks can be monitored. Avoid sensitive logins, and use a VPN if you must stay connected."),

                MultipleChoice(
                    "What is two-factor authentication (2FA)?",
                    new[] { "Using two different passwords", "Proving your identity with a second step, like a one-time code, as well as your password", "Logging in from two devices", "Having two email accounts" },
                    1,
                    "2FA means a stolen password alone isn't enough for someone to get into your account."),

                MultipleChoice(
                    "Which type of malware locks your files and demands payment to unlock them?",
                    new[] { "Spyware", "Adware", "Ransomware", "A worm" },
                    2,
                    "Ransomware encrypts your files. Regular backups are your best protection against it."),

                TrueFalse(
                    "Using the same password on all your accounts is fine as long as the password is strong.",
                    false,
                    "If one site is breached, attackers try that password everywhere else. Use a unique password for every account."),

                TrueFalse(
                    "Your bank will never ask for your PIN or full password by email or SMS.",
                    true,
                    "Legitimate banks never request secret details this way. A message that does is almost certainly a scam."),

                TrueFalse(
                    "Software updates only add new features and are not important for security.",
                    false,
                    "Updates often fix security holes that attackers actively exploit, so install them promptly."),

                TrueFalse(
                    "If a website looks professional, it is definitely safe.",
                    false,
                    "Scammers can copy real designs easily. Check the web address and how you arrived at the site."),

                TrueFalse(
                    "Backing up your important files helps you recover after a ransomware attack.",
                    true,
                    "With a recent backup stored safely, you can restore your files without paying criminals.")
            };
        }

        private static QuizQuestion MultipleChoice(string text, string[] options, int correctIndex, string explanation)
        {
            return new QuizQuestion
            {
                Text = text,
                Options = options.ToList(),
                CorrectIndex = correctIndex,
                Explanation = explanation
            };
        }

        private static QuizQuestion TrueFalse(string text, bool answer, string explanation)
        {
            return new QuizQuestion
            {
                Text = "True or False: " + text,
                Options = new List<string> { "True", "False" },
                CorrectIndex = answer ? 0 : 1,
                Explanation = explanation
            };
        }
    }
}