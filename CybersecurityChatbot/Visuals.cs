using System;
using System.IO;
using System.Media;
using System.Threading;

namespace CybersecurityChatbot
{
    public static class Visuals
    {
        // Setup console window theme colors
        public static void SetTheme()
        {
            Console.Title = "Cybersecurity Awareness Assistant";
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        // Question 1: Audio Playback (Plays your recorded WAV file)
        public static void PlayVoiceGreeting(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using (SoundPlayer player = new SoundPlayer(fileName))
                    {
                        player.Play(); // Plays in background
                    }
                }
            }
            catch
            {
                // Fallback gracefully if computer sound is disabled
            }
        }

        // Question 2: Cybersecurity-Themed ASCII Shield Art
        public static void DisplayAsciiLogo()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"      __________________      ");
            Console.WriteLine(@"     /                  \     ");
            Console.WriteLine(@"    |   [SECURE SHIELD]  |    ");
            Console.WriteLine(@"    |     _________      |    ");
            Console.WriteLine(@"    |    |  /\ /\  |     |    ");
            Console.WriteLine(@"    |    |  \_/    |     |    ");
            Console.WriteLine(@"     \    \_______/     /     ");
            Console.WriteLine(@"      \                /      ");
            Console.WriteLine(@"       \______________/       ");
            Console.WriteLine("  CYBERSECURITY AWARENESS BOT ");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        // Question 6: Typing Simulation Animation Effect
        public static void TypeText(string text, ConsoleColor color = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = color;
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(15); // Tiny micro-delay to mimic human typing
            }
            Console.WriteLine();
        }

        // Section header separation helper
        public static void ShowHelpMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n==================================================");
            Console.WriteLine("               AVAILABLE TOPICS                   ");
            Console.WriteLine("==================================================");
            Console.WriteLine("* Chat: 'How are you?' or 'What is your purpose?'");
            Console.WriteLine("* Advice: 'password safety', 'phishing', 'safe browsing'");
            Console.WriteLine("* System: Type 'exit' to end session.");
            Console.WriteLine("==================================================\n");
        }
    }
}
