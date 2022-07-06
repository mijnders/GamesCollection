namespace GamesCollection.Domain.Wizard
{
    public class StartWizard
    {
        private static readonly string[] Species = { "Human", "Dwarf", "Elves", "Giant" };
        internal static readonly List<WizardCard> Cards = new();
        internal static List<WizardCard[]> HandCardsList = new();
        internal static List<int> predictInts = new List<int>();
        static bool inputInvalid;
        public static void Start()
        {
            CardRestock();
            while (true)
            {
                var chosenShapes = new string[2];
                Console.Write(Translator.Translate("Do you want to play against the "));
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(Translator.Translate("Computer") + "(c)");
                Console.ResetColor();
                Console.Write(Translator.Translate(" or against another "));
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(Translator.Translate("Player") + "(p)");
                Console.ResetColor();
                Console.WriteLine("?");
                Console.Write(Translator.Translate("Enter "));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Translator.Translate("Rules") + "(r)");
                Console.ResetColor();
                Console.Write(Translator.Translate(" to view the rules or "));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Translator.Translate("Exit") + "(x)");
                Console.ResetColor();
                Console.Write(Translator.Translate(" to end the game."));
                Console.WriteLine();
                if (inputInvalid) InputInvalid();
                Console.Write(Translator.Translate("Your choice: "));
                var chosenOpponent = Console.ReadLine();
                if (chosenOpponent != null)
                    switch (Translator.TranslateBackwards(chosenOpponent.Replace(" ", "")).ToLower())
                    {
                        case "player":
                        case "p":
                            PlayAgainstPlayer();
                            break;
                        case "computer":
                        case "c":
                            break;
                        case "rules":
                        case "r":
                            break;
                        case "exit":
                        case "x":
                            Console.Clear();
                            WhenReset("Wizard", 0);
                            return;
                        default:
                            Console.Clear();
                            WhenReset("Wizard", 0);
                            inputInvalid = true;
                            break;
                    }
                Console.WriteLine();
                Console.WriteLine(Translator.Translate("Press Enter to continue"));
                Console.ReadKey();
                Console.Clear();
                WhenReset("Wizard", 0);
            }

        }

        private static void PlayAgainstPlayer()
        {
            var round = 1;
            var plop = true;
            while (plop)
            {
                Console.WriteLine(Translator.Translate("How many players are you?"));
                var input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out var result))
                {
                    for (var i = 0; i < Cards.Count / result; i++)
                    {
                        HandOutCards(result, round);
                        CardRestock();
                        round++;
                    }
                    plop = false;
                }
                else
                {
                    Console.Clear();
                    WhenReset("Wizard", 0);
                    InputInvalid();
                    plop = true;
                }
            }
        }
        private static void HandOutCards(int quantity, int round)
        {
            HandCardsList = new List<WizardCard[]>();
            var random = new Random();
            for (var i = 0; i < round; i++)
            {
                var cards = new WizardCard[quantity];
                for (var j = 0; j < quantity; j++)
                {
                    cards[j] = Cards[random.Next(Cards.Count)];
                    Cards.Remove(cards[j]);
                }
                HandCardsList.Add(cards);
            }
            var topCard = Cards[random.Next(Cards.Count)];
            Cards.Remove(topCard);
            Console.Clear();
            for (var i = 0; i < quantity; i++)
            {
                WhenReset("Wizard", 0);
                Console.WriteLine();
                Console.Write($"Stack:\t\t");
                Console.ForegroundColor = topCard.Color;
                Console.Write(topCard.Species[..1] + topCard.Value);
                Console.ResetColor();
                Console.WriteLine();
                WhenReset("Player", i + 1);
                Console.WriteLine();
                Console.Write("Player ");
                Console.Write(i + 1);
                Console.Write(" press Enter");
                Console.ReadKey();
                Console.Clear();
                WhenReset("Wizard", 0);
                Console.WriteLine();
                Console.Write($"Stack:\t\t");
                Console.ForegroundColor = topCard.Color;
                Console.Write(topCard.Species[..1] + topCard.Value);
                Console.ResetColor();
                Console.WriteLine();
                WhenReset("Player", i + 1);
                Console.WriteLine();
                Console.Write($"on hand:\t");
                foreach (var card in HandCardsList.Select(cards => cards[i]))
                {
                    Console.ForegroundColor = card.Color;
                    switch (card.Value)
                    {
                        case 0:
                            Console.Write("N ");
                            break;
                        case 14:
                            Console.Write("Z ");
                            break;
                        default:
                            Console.Write(card.Species[..1] + card.Value + " ");
                            break;
                    }
                    Console.ResetColor();
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(Translator.Translate("Please make your prediction"));
                Console.Write(Translator.Translate("Remember that only "));
                Console.Write(round);
                if(round <= 1) Console.Write(Translator.Translate(" trick is possible"));
                else Console.WriteLine(Translator.Translate(" tricks are possible"));
                Console.WriteLine();
                Console.Write(Translator.Translate("Your Choice: "));
                var input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input) | int.TryParse(input, out int result))
                {
                    predictInts.Add(result);
                }
                Console.Clear();
            }
        }

        private static void CardRestock()
        {
            if (Cards.Count != 0)
            {
                Cards.Clear();
            }
            foreach (var specy in Species)
            {
                for (var i = 0; i < 15; i++)
                {
                    Cards.Add(new WizardCard(specy, i));
                }
            }
        }

        private static void InputInvalid()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Translator.Translate("Your last entry was invalid. Please try again"));
            Console.ResetColor();
        }
        private static void WhenReset(string title, int player)
        {
            var playerInt = player != 0 ? player.ToString() : string.Empty;
            Console.ResetColor();
            var width = (Console.WindowWidth - (Translator.Translate(title) + " " + playerInt).Length) / 2;
            for (var i = 0; i < width; i++) Console.Write("-");
            Console.Write(Translator.Translate(title) + " " + playerInt);
            for (var i = 0; i < width; i++) Console.Write("-");
            Console.WriteLine();
        }
    }
}
