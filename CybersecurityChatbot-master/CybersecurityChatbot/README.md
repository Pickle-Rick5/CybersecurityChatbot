# Cybersecurity Awareness Chatbot — PROG6221 POE

A console and GUI-based chatbot designed to educate South African citizens on common cybersecurity threats, including phishing, password safety, safe browsing, scams, and privacy protection.

## Project Structure

This solution contains two projects, one per part of the POE:

| Project | Description |
|---|---|
| `CybersecurityChatbot` | **Part 1** — Console application. Core chatbot logic, ASCII art, voice greeting, and keyword-based responses. |
| `CybersecurityChatbotPart2` | **Part 2** — WPF GUI application. Expands Part 1 with a graphical interface, keyword recognition, random response variation, conversation flow, memory/recall, and sentiment detection. |

Both projects are preserved independently — Part 1 remains untouched from its original submission.

## Requirements

- Windows OS (required for WPF and the audio playback features)
- .NET 10 SDK or later
- Visual Studio 2022+ (Community edition is sufficient)

## How to Run — Part 1 (Console App)

1. Open `CybersecurityChatbot.sln` in Visual Studio
2. In Solution Explorer, right-click the **`CybersecurityChatbot`** project → **Set as Startup Project**
3. Press **F5** (or Ctrl+F5 to run without debugging)
4. Follow the on-screen prompts: enter your name, then ask about topics like `password`, `phishing`, or `browsing`. Type `exit` to quit.

## How to Run — Part 2 (WPF GUI App)

1. Open `CybersecurityChatbot.sln` in Visual Studio
2. In Solution Explorer, right-click the **`CybersecurityChatbotPart2`** project → **Set as Startup Project**
3. Press **F5** (or Ctrl+F5 to run without debugging)
4. The chatbot window will open with a voice greeting and ASCII-style header
5. Type your name when prompted, then chat naturally — try:
   - `Tell me about password safety`
   - `Give me a phishing tip` → then `give me another tip` to see varied responses on the same topic
   - `I'm interested in privacy` → the bot will remember this and reference it later
   - `I'm worried about scams` → the bot responds with empathy before giving a tip
   - `exit` to end the session

## Features Implemented (Part 2)

- **GUI design** — dark-themed WPF interface with styled chat log, carried-over ASCII branding, and working voice greeting (via `System.Media.SoundPlayer`)
- **Keyword recognition** — recognizes 5 core topics: password, phishing, browsing, scam, privacy
- **Random responses** — each topic has multiple response variants, randomly selected to avoid repetition
- **Conversation flow** — supports natural follow-ups ("tell me more", "give me another tip") without needing to restate the topic
- **Memory and recall** — tracks the user's most-discussed topic and personalises later responses accordingly
- **Sentiment detection** — recognizes simple moods (worried, frustrated, curious) and responds with empathy alongside the actual cybersecurity tip
- **Error handling** — graceful fallback response for unrecognized input; no crashes on empty or unexpected input

## Notes on Audio

`welcome.wav` must be present in the output directory alongside the `.exe` (this is handled automatically via "Copy to Output Directory" = "Copy if newer" in the project settings). The WAV file must be in standard 16-bit PCM format — non-PCM WAV files will fail to play via `SoundPlayer`.

## Author

[Jason Adams / ST10435412]