using System;

namespace CybersecurityChatbot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialization
            Visuals.SetTheme();
            Visuals.PlayVoiceGreeting("welcome.wav"); // Audio trigger
            Visuals.DisplayAsciiLogo(); // Visual Header Display

            Visuals.TypeText("Initializing communication lines...", ConsoleColor.DarkGray);
            Visuals.TypeText("Chatbot: Hello! Please enter your name to start our safety briefing: ");

            Console.ForegroundColor = ConsoleColor.White;
            string name = Console.ReadLine();

            // Question 5: Continuous Name Input Validation
            while (string.IsNullOrWhiteSpace(name))
            {
                Visuals.TypeText("Chatbot: Entry invalid. Please type a valid name: ", ConsoleColor.Red);
                Console.ForegroundColor = ConsoleColor.White;
                name = Console.ReadLine();
            }

            Visuals.TypeText($"\nWelcome, {name}! Let's review common web safety patterns.", ConsoleColor.Cyan);
            Visuals.ShowHelpMenu();

            ChatEngine engine = new ChatEngine(name);
            bool exitSignal = false;

            // Chat Interface Interaction Loop
            while (!exitSignal)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n{name} > ");
                Console.ForegroundColor = ConsoleColor.White;
                string input = Console.ReadLine();

                // Question 5: Empty Input Validation Check
                if (string.IsNullOrWhiteSpace(input))
                {
                    Visuals.TypeText("Chatbot: Input cannot be empty. Please ask a question.", ConsoleColor.Red);
                    continue;
                }

                string response = engine.GetResponse(input, out exitSignal);
                Visuals.TypeText($"Chatbot: {response}", exitSignal ? ConsoleColor.DarkYellow : ConsoleColor.Cyan);
            }
        }
    }
}
