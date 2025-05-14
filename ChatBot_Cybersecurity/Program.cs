using System;
using System.Media;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot_Cybersecurity
{
    class ChatBot
    {
        private static string userName = " "; //null in the beginning
        private static string userInterest = ""; // For memory feature
        private static string favoriteTopic = ""; //To save favourite topic
        private static bool isWorried = false;   // For sentiment detection
        private static bool isCurious = false;   // For sentiment detection
        private static bool isFrustrated = false;   // For sentiment detection
        private static bool isFavourite = false;   // For sentiment detection

        // Colors for text
        private static ConsoleColor defaultColor = ConsoleColor.White;
        private static ConsoleColor botColor = ConsoleColor.Cyan;
        private static ConsoleColor userColor = ConsoleColor.Green;
        private static ConsoleColor warningColor = ConsoleColor.Yellow;
        private static ConsoleColor errorColor = ConsoleColor.White;
        private static ConsoleColor positiveColor = ConsoleColor.Magenta;

        //set exit boolean to false
        private static bool exitRequested = false;

        // Arrays: Response collections for random selection
        private static List<string> passwordResponses = new List<string>
        {
            "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.",
            "A good password should be long (12+ characters) and include a mix of letters, numbers, and symbols.",
            "Consider using a passphrase like 'BlueCoffee$Makes5Cups!' instead of a simple password.",
            "Never share your passwords with anyone, even if they claim to be from tech support."
        };

        private static List<string> phishingResponses = new List<string>
        {
            "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
            "Always check the sender's email address - phishing emails often come from addresses that look similar but not identical to legitimate ones.",
            "If an email creates a sense of urgency or threatens consequences, it's likely a phishing attempt.",
            "Hover over links before clicking to see the actual URL. If it looks suspicious, don't click!"
        };

        private static List<string> browsingResponses = new List<string>
        {
            "Always look for HTTPS and the padlock icon in your browser when entering sensitive information.",
            "Keep your browser and all plugins updated to protect against known vulnerabilities.",
            "Use browser extensions that block malicious websites and trackers for safer browsing.",
            "Be especially careful when using public Wi-Fi - consider using a VPN for added security."
        };

        static void Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Bot";
            PlayWelcomeAudio(); //play welcome audio
            DisplayAsciiArt(); //display ascii art
            GetUserName(); //get user name

            while (!exitRequested) //chatbot loop
            {
                Console.WriteLine();
                TypeWrite("Which cybersecurity topic(s) can I help you with today?", botColor);
                Console.Write("> ");
                Console.ForegroundColor = userColor;
                string input = Console.ReadLine();
                Console.ForegroundColor = defaultColor;

                ProcessInput(input);
            }

            TypeWrite($"Goodbye, {userName}! Stay safe online!", botColor);
            Thread.Sleep(2000);
        }

        private static void ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                TypeWrite("I didn't quite understand that. Could you rephrase?", botColor);
                return;
            }

            string lowerInput = input.ToLower();

            // Sentiment detection
            DetectSentiment(lowerInput);

            //Save favourite topic
            CheckForFavoriteTopic(lowerInput);

            // Keyword recognition with conversation flow
            if (lowerInput.Contains("hello") || lowerInput.Contains("hi") || lowerInput.Contains("hey"))
            {
                if (isWorried && lowerInput.Contains("scam") || lowerInput.Contains("phishing"))
                {
                    TypeWrite("It's totally understandable to feel that way. Scammers can be convinving for those unfamiliar with cybersecurity. Let me help you stay safe.", botColor);
                                          
                }
                TypeWrite(lowerInput, botColor);
            }
            else if (lowerInput.Contains("how are you"))
            {
                TypeWrite("I'm just a bot, but I'm working perfectly! Ready to help you with cybersecurity.", botColor);
            }
            else if (lowerInput.Contains("purpose") || lowerInput.Contains("what do you do"))
            {
                TypeWrite("My purpose is to help you stay safe online by providing cybersecurity awareness.", botColor);
                TypeWrite("I can explain common threats and best practices for staying secure.", botColor);
            }
            else if (ContainsKeywords(lowerInput, new[] { "password", "login", "credential" }))
            {
                userInterest = "password safety";
                DisplayPasswordSafetyAnswers();
            }
            else if (ContainsKeywords(lowerInput, new[] { "phishing", "scam", "fraud", "email scam" }))
            {
                userInterest = "phishing protection";
                DisplayPhishingAnswers(input);
            }
            else if (ContainsKeywords(lowerInput, new[] { "browsing", "internet", "online", "privacy", "browser" }))
            {
                userInterest = "safe browsing";
                DisplaySafeBrowsingInfo(input);
            }
            else if (lowerInput.Contains("thank") || lowerInput.Contains("thanks"))
            {
                TypeWrite($"You're welcome, {userName}! Is there anything else you'd like to know about {(!string.IsNullOrEmpty(userInterest) ? userInterest : "cybersecurity")}?", positiveColor);
            }
            else if (lowerInput.Contains("more") || lowerInput.Contains("detail") || lowerInput.Contains("explain"))
            {
                ProvideMoreDetails();
            }
            else if (lowerInput.Contains("exit") || lowerInput.Contains("quit") || lowerInput.Contains("bye"))
            {
                exitRequested = true;
            }
            else if (lowerInput.Contains("favorite topic") || lowerInput.Contains("favourite topic"))
            {
                if (!string.IsNullOrEmpty(favoriteTopic))
                {
                    TypeWrite($"Your favorite topic is {favoriteTopic}, {userName}!", positiveColor);
                    // Provide information about the favorite topic
                    switch (favoriteTopic.ToLower())
                    {
                        case "password safety":
                            DisplayPasswordSafetyAnswers();
                            break;
                        case "phishing protection":
                            DisplayPhishingAnswers(input);
                            break;
                        case "safe browsing":
                            DisplaySafeBrowsingInfo(input);
                            break;
                    }
                }
                else
                {
                    TypeWrite($"You haven't told me your favorite topic yet, {userName}. You can say something like 'password safety is my favorite topic'.", botColor);
                }
            }
            else
            {
                HandleUnknownInput();
            }

        }

        private static bool ContainsKeywords(string input, string[] keywords)
        {
            return keywords.Any(keyword => input.Contains(keyword));
        }

        private static void DetectSentiment(string input)
        {
            isWorried = input.Contains("worried") || input.Contains("concern") ||
                        input.Contains("scared") || input.Contains("anxious");

            isCurious = input.Contains("curious") || input.Contains("interested");

            isFrustrated = input.Contains("frustrated") || input.Contains("angry") ||
                        input.Contains("upset") || input.Contains("disappointed");

            isFavourite = input.Contains("favourite") || input.Contains("like");
                        
        }

        private static void CheckForFavoriteTopic(string input)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("favorite") || lowerInput.Contains("favourite"))
            {
                if (lowerInput.Contains("password") || lowerInput.Contains("login") || lowerInput.Contains("credential"))
                {
                    favoriteTopic = "password safety";
                    TypeWrite($"I've noted that password safety is your favorite topic, {userName}!", positiveColor);
                }
                else if (lowerInput.Contains("phishing") || lowerInput.Contains("scam") || lowerInput.Contains("fraud"))
                {
                    favoriteTopic = "phishing protection";
                    TypeWrite($"I've noted that phishing protection is your favorite topic, {userName}!", positiveColor);
                }
                else if (lowerInput.Contains("browsing") || lowerInput.Contains("internet") || lowerInput.Contains("online") || lowerInput.Contains("privacy"))
                {
                    favoriteTopic = "safe browsing";
                    TypeWrite($"I've noted that safe browsing is your favorite topic, {userName}!", positiveColor);
                }
            }
        } 

        private static void ProvideMoreDetails()
        {
            if (!string.IsNullOrEmpty(userInterest))
            {
                switch (userInterest.ToLower())
                {
                    case "password safety":
                        TypeWrite("Here's more about password safety:", botColor);
                        TypeWrite(GetRandomResponse(passwordResponses), botColor);
                        break;
                    case "phishing protection":
                        TypeWrite("Additional phishing information:", botColor);
                        TypeWrite(GetRandomResponse(phishingResponses), botColor);
                        break;
                    case "safe browsing":
                        TypeWrite("More safe browsing tips:", botColor);
                        TypeWrite(GetRandomResponse(browsingResponses), botColor);
                        break;
                    default:
                        TypeWrite("What specific topic would you like more information about?", botColor);
                        break;
                }
            }
            else
            {
                TypeWrite("Which topic would you like me to explain in more detail?", botColor);
            }
        }

        private static void HandleUnknownInput() //Error handling
        {
            if (!string.IsNullOrEmpty(userInterest))
            {
                TypeWrite($"I'm not sure I understand. Would you like more information about {userInterest}?", botColor);
            }
            else
            {
                TypeWrite("I didn't quite understand that. I can help with:", botColor);
                TypeWrite("- PASSWORD SAFETY (try 'tell me about passwords')", botColor);
                TypeWrite("- PHISHING SCAMS (try 'how to spot scams')", botColor);
                TypeWrite("- SAFE BROWSING PRACTICES (try 'internet safety tips')", botColor);
            }
        }

        private static string GetRandomResponse(List<string> responses) //Selects random responses from the Array list
        {
            Random rnd = new Random();
            return responses[rnd.Next(responses.Count)];
        }

        private static void DisplayPasswordSafetyAnswers()
        {
            Console.ForegroundColor = botColor;
            Console.WriteLine("\n" + new string('=', 40) + " PASSWORD SAFETY " + new string('=', 40) + "\n");
            TypeWrite(GetRandomResponse(passwordResponses), botColor);

            if (isWorried)
            {
                TypeWrite("Don't worry, with strong passwords you're already ahead of most threats!", positiveColor);
                TypeWrite(GetRandomResponse(passwordResponses), botColor);
            }

            if (isCurious)
            {
                TypeWrite("It's good to be curious as we can learn more! Here's more tips for you on password safety!", botColor);
                Console.ForegroundColor = botColor;
                Console.WriteLine("\n" + new string('=', 40) + " PASSWORD SAFETY " + new string('=', 40) + "\n");

                TypeWrite("Creating strong passwords is crucial for online security. Here are some tips:", botColor);
                Console.WriteLine();

                TypeWrite("1. Use long passwords (at least 10 characters)", botColor);
                TypeWrite("2. Include uppercase, lowercase, numbers, and special characters", botColor);
                TypeWrite("3. Don't use personal information like birthdays or names", botColor);
                TypeWrite("4. Use a unique password for each account", botColor);
                TypeWrite("5. Consider using a password manager to keep track", botColor);
                TypeWrite("6. Enable two-factor authentication where available", botColor);

                Console.WriteLine("\n" + new string('-', 60));
                TypeWrite("Example of a strong password: 'Blue42$ky!Turtle@9'", botColor);
                TypeWrite("Example of a weak password: 'password0000'", warningColor);

                Console.WriteLine("\n" + new string('=', 97) + "\n");
                Console.ForegroundColor = defaultColor;

            }
            else if (isFavourite)
            {
                TypeWrite("Great! I'll remember that this is your favourite topic!", botColor);
            }

            Console.WriteLine("\n" + new string('=', 97) + "\n");
            Console.ForegroundColor = defaultColor;
        }

        private static void DisplayPhishingAnswers(string input)
        {
            Console.ForegroundColor = botColor;
            Console.WriteLine("\n" + new string('=', 38) + " PHISHING PROTECTION " + new string('=', 38) + "\n");
            TypeWrite(GetRandomResponse(phishingResponses), botColor);

            string lowerInput = input.ToLower();

            if (isWorried)
            {
                TypeWrite("Remember, being cautious is good, but don't let fear stop you from enjoying the internet!", positiveColor);
                TypeWrite("Let me know if you would like to gain more tips on scam/phishing.", positiveColor);
                if (lowerInput.Contains("yes"))
                {
                    Console.WriteLine("Here are more tips for you!");
                    Console.ForegroundColor = botColor;
                    Console.WriteLine("\n" + new string('=', 38) + " PHISHING PROTECTION " + new string('=', 38) + "\n");

                    TypeWrite("Phishing is when attackers try to trick you into giving sensitive information.", botColor);
                    Console.WriteLine();

                    TypeWrite("How to spot phishing attempts:", botColor);
                    TypeWrite("1. Urgent or threatening language demanding immediate action", botColor);
                    TypeWrite("2. Requests for personal information via email or message", botColor);
                    TypeWrite("3. Suspicious sender addresses (hover to check before clicking)", botColor);
                    TypeWrite("4. Poor spelling and grammar", botColor);
                    TypeWrite("5. Links that don't match the supposed sender (hover to check)", botColor);
                    TypeWrite("6. Unexpected attachments", botColor);

                    Console.WriteLine("\n" + new string('-', 60));
                    TypeWrite("Always verify requests through official channels before responding.", botColor);
                    TypeWrite("When in doubt, don't click! Contact the organisation directly.", warningColor);

                    
                }
            if (isCurious)
                {
                    Console.ForegroundColor = botColor;
                    Console.WriteLine("\n" + new string('=', 38) + " PHISHING PROTECTION " + new string('=', 38) + "\n");

                    TypeWrite("Phishing is when attackers try to trick you into giving sensitive information.", botColor);
                    Console.WriteLine();

                    TypeWrite("How to spot phishing attempts:", botColor);
                    TypeWrite("1. Urgent or threatening language demanding immediate action", botColor);
                    TypeWrite("2. Requests for personal information via email or message", botColor);
                    TypeWrite("3. Suspicious sender addresses (hover to check before clicking)", botColor);
                    TypeWrite("4. Poor spelling and grammar", botColor);
                    TypeWrite("5. Links that don't match the supposed sender (hover to check)", botColor);
                    TypeWrite("6. Unexpected attachments", botColor);

                    Console.WriteLine("\n" + new string('-', 60));
                    TypeWrite("Always verify requests through official channels before responding.", botColor);
                    TypeWrite("When in doubt, don't click! Contact the organisation directly.", warningColor);

                }
            if (isFavourite)
                {
                    TypeWrite("Great! I'll remember that this is your favourite topic!", botColor);
                }
                else
                {
                    Console.WriteLine("\n" + new string('=', 97) + "\n");
                    Console.ForegroundColor = defaultColor;
                }
                
            }

            
        }

        private static void DisplaySafeBrowsingInfo(string input)
        {
            Console.ForegroundColor = botColor;
            Console.WriteLine("\n" + new string('=', 37) + " SAFE BROWSING PRACTICES " + new string('=', 37) + "\n");

            TypeWrite(GetRandomResponse(browsingResponses), botColor);

            if (isWorried)
            {
                TypeWrite("The internet is generally safe if you follow good cyber practices. No need to worry too much!", positiveColor);
                TypeWrite(GetRandomResponse(browsingResponses), botColor);
            }

            if (isCurious)
            {
                TypeWrite("Curiosity is good when it brings to discover more things!", positiveColor);
                TypeWrite(GetRandomResponse(browsingResponses), botColor);
                if (input.Contains("yes"))
                {
                    Console.ForegroundColor = botColor;
                    Console.WriteLine("\n" + new string('=', 37) + " SAFE BROWSING PRACTICES " + new string('=', 37) + "\n");

                    TypeWrite("Staying safe while browsing the internet:", botColor);
                    Console.WriteLine();

                    TypeWrite("1. Look for 'https://' and the padlock icon in your browser", botColor);
                    TypeWrite("2. Keep your browser and operating system updated", botColor);
                    TypeWrite("3. Use reputable antivirus and anti-malware software", botColor);
                    TypeWrite("4. Be cautious with downloads - only from trusted sources", botColor);
                    TypeWrite("5. Use a VPN on public Wi-Fi networks", botColor);
                    TypeWrite("6. Regularly clear cookies and cache", botColor);
                    TypeWrite("7. Be careful of 'too good to be true' offers", botColor);

                    Console.WriteLine("\n" + new string('-', 60));
                    TypeWrite("Remember: If something feels suspicious, it probably is!", warningColor);

                    
                }
                else if(isFavourite)
                {
                    TypeWrite("Great! I'll remember that this is your favourite topic!", botColor);
                }
                else
                {

                    Console.WriteLine("\n" + new string('=', 97) + "\n");
                    Console.ForegroundColor = defaultColor;
                }
            }
        }
        private static void PlayWelcomeAudio()
        {
            try
            {//retrieve audio wav file
                string path = "WAVChatbotWelcomeAudio.wav";//Path.Combine(Directory.GetCurrentDirectory(), "Resources", "WAVChatbotWelcomeAudio.wav");
                if (File.Exists(path))  //play sound when audio is found
                {
                    using (SoundPlayer player = new SoundPlayer(path))
                    {
                        player.PlaySync();
                        // Wait for the greeting to finish playing
                        //Thread.Sleep(3000);
                    }
                }
                else //continue program even when audio can't play
                {
                    Console.ForegroundColor = warningColor;
                    Console.WriteLine("Welcome sound file not found. Continuing without audio...");
                    Console.ForegroundColor = defaultColor;
                }
            }
            catch (Exception ex) //continue program even when audio can't play
            {
                Console.ForegroundColor = errorColor;
                Console.WriteLine($"Error playing welcome sound: {ex.Message}");
                Console.ForegroundColor = defaultColor;
            }
        }

        private static void DisplayAsciiArt() //display ascii art logo
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(@"
  ____      _                                        _ _            
 / ___|   _| |__   ___ _ __ ___  ___  ___ _   _ _ __(_) |_ _   _    
| |  | | | | '_ \ / _ \ '__/ __|/ _ \/ __| | | | '__| | __| | | |   
| |__| |_| | |_) |  __/ |  \__ \  __/ (__| |_| | |  | | |_| |_| |   
 \____\__, |_.__/ \___|_|  |___/\___|\___|\__,_|_|  |_|\__|\__, |   
   / \|___/   ____ _ _ __ ___ _ __   ___  ___ ___  | __ )  |___/ |_ 
  / _ \ \ /\ / / _` | '__/ _ \ '_ \ / _ \/ __/ __| |  _ \ / _ \| __|
 / ___ \ V  V / (_| | | |  __/ | | |  __/\__ \__ \ | |_) | (_) | |_ 
/_/   \_\_/\_/ \__,_|_|  \___|_| |_|\___||___/___/ |____/ \___/ \__|
    
 ");
            Console.ForegroundColor = defaultColor;
            Console.WriteLine("\n" + new string('=', 80) + "\n");
        }
        private static void GetUserName() //prompt user input for name
        {
            while (string.IsNullOrWhiteSpace(userName))
            {
                TypeWrite("Hello! Before we begin, what's your name?", botColor); //ask user for name
                Console.Write("> ");
                Console.ForegroundColor = userColor;
                userName = Console.ReadLine(); //use the name to personalise response
                Console.ForegroundColor = defaultColor;

                if (string.IsNullOrWhiteSpace(userName)) //blank username input
                {
                    Console.ForegroundColor = warningColor;
                    TypeWrite("I didn't catch your name. Could you please tell me your name?", botColor);
                    Console.ForegroundColor = defaultColor;
                }
                else
                {
                    Console.WriteLine();
                    TypeWrite($"Nice to meet you, {userName}! I'm your Cybersecurity Awareness ChatBot.", botColor);
                    TypeWrite("I can help you with topics like PASSWORD SAFETY, PHISHING, and SAFE BROWSING.", botColor);
                    TypeWrite("Reply with 'password' / 'scam' / 'browsing'", botColor);
                    Console.WriteLine(new string('-', 60));
                }
            }
        }
        private static void TypeWrite(string message, ConsoleColor color)  //typing effect: display message letter by letter
        {
            Console.ForegroundColor = color;
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(20); //pauses after message is displayed
            }
            Console.WriteLine();
            Console.ForegroundColor = defaultColor;
        }


    }
}