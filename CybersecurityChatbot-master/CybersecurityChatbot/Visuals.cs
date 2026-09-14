using System;
using System.IO;
using System.Runtime.InteropServices; // Required to use native Windows Audio components
using System.Text;
using System.Threading;

namespace CybersecurityChatbot
{
    public static class Visuals
    {
        // Import the native Windows Media command string player to handle all WAV types seamlessly
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, IntPtr hwndCallback);

        // Setup console window theme colors
        public static void SetTheme()
        {
            Console.Title = "Cybersecurity Awareness Assistant";
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        // Question 1: Audio Playback (Plays any WAV file format using native Windows Multimedia)
        public static void PlayVoiceGreeting(string fileName)
        {
            try
            {
                // This looks directly inside the folder where your CybersecurityChatbot.exe sits
                string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                if (File.Exists(absolutePath))
                {
                    // Open the audio file using short file paths to prevent space character syntax issues
                    mciSendString($"open \"{absolutePath}\" type waveaudio alias welcomeVoice", null, 0, IntPtr.Zero);

                    // Play the audio file synchronously so it finishes before text rendering begins
                    mciSendString("play welcomeVoice wait", null, 0, IntPtr.Zero);

                    // Close the audio stream path to clean up memory footprint resources
                    mciSendString("close welcomeVoice", null, 0, IntPtr.Zero);
                }
            }
            catch
            {
                // Suppress failure paths safely if audio devices are missing
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
