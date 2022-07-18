using static GamesCollection.Domain.Wizard.WizardLogic;

namespace GamesCollection.Domain.Wizard;

internal class WizardDomain
{
    private static List<WizardPlayer> WizardPlayers { get; set; }
    private static string? TrumpSpecies { get; set; }
    private static WizardCardDeck Stack { get; set; }
    private static WizardCardDeck MainCardDeck { get; set; }
    public static void StartPoint()
    {
        var playerCount = GetPlayerCount();
        WizardPlayers = CreatePlayers(playerCount, GetPlayersNames(playerCount));
        for (var stitchRound = 3; stitchRound <= (60 / playerCount); stitchRound++)
        {
            MainCardDeck = CreateMainDeck();
            var wizardCardsForHands = HandsOutCards(stitchRound, playerCount, MainCardDeck);
            var firstPlayer = WizardPlayers[(stitchRound - 1) % playerCount];
            TrumpSpecies = GetTrumpSpecies(MainCardDeck);
            Stack = new WizardCardDeck
            {
                Deck = new List<WizardCard>()
            };
            var modulo = (stitchRound - 1) % playerCount;
            WizardPlayers = SortWizardPlayers(new List<WizardPlayer>(WizardPlayers.OrderBy(item => item.Id)), modulo);
            for (var playerIndex = 0; playerIndex < playerCount; playerIndex++)
            {
                WizardPlayers[playerIndex].OnHandCards = wizardCardsForHands[playerIndex];
                if (!string.IsNullOrEmpty(TrumpSpecies) && TrumpSpecies == "Wizard") TrumpSpecies = ChooseTrumpSpecies(firstPlayer, TrumpSpecies);
                WizardPlayers[playerIndex].Prediction = GetPrediction(WizardPlayers[playerIndex]);
            }
            for (var round = 0; round < stitchRound; round++)
            {
                var playedBy = new List<int>();
                foreach (var currentPlayer in WizardPlayers)
                {
                    var card = PlayCard(currentPlayer);
                    Stack.Deck.Add(card);
                    currentPlayer.OnHandCards.Remove(card);
                    playedBy.Add(WizardPlayers.ToList().IndexOf(currentPlayer));
                }

                var indexOfPlayer = CheckPlayedCards(playedBy, Stack, TrumpSpecies);
                if (indexOfPlayer != -1 && indexOfPlayer < WizardPlayers.Count) WizardPlayers[indexOfPlayer].Tricks++;
                Stack.Deck.Clear();
                WizardPlayers = SortWizardPlayers(WizardPlayers, indexOfPlayer);
            }

            foreach (var player in WizardPlayers)
            {
                CalculateExp(player);
            }
            Console.WriteLine();
            ShowIntermediate();
        }

        Console.ReadKey();
    }

    private static void ShowIntermediate()
    {
        BuildUpInterface();
        Program.ClearConsole("Intermediate", false);
        Console.WriteLine();
        foreach (var player in WizardPlayers)
        {
            Console.Write($"{player.Name}: {player.Tricks}/{player.Prediction}");
            if(player != WizardPlayers.Last()) Console.Write(";\t");
            player.Tricks = 0;
            player.Prediction = 0;
        }
        Console.WriteLine();
        Console.WriteLine();
        foreach (var player in WizardPlayers)
        {
            Console.Write($"{player.Name}: {player.ExperiencePoints}");
            if (player != WizardPlayers.Last() && player.ExperiencePoints.ToString().Length > 2) Console.Write(";\t");
            else if (player != WizardPlayers.Last()) Console.Write(";\t\t");
        }
        Console.WriteLine();
        Console.ReadKey();
    }

    private static WizardCard PlayCard(WizardPlayer currentPlayer)
    {
        WizardCard? card = null;
        do
        {
            BuildUpInterface();
            BuildUpPlayerInterface(currentPlayer);
            if (!Program.IsValid) Program.FalseInput();
            Console.Write(Translator.Translate("Playing") + ":\t");
            var input = Console.ReadLine();
            card = CheckCard(currentPlayer.OnHandCards, input);
            string? firstSpecies = null;
            if (Stack.Deck.Count > 0)
            {
                if (Stack.Deck.First().Value is 14 or 0)
                {
                    foreach (var wizardCard in Stack.Deck.Where(wizardCard => wizardCard.Value != 14 | wizardCard.Value != 0))
                    {
                        firstSpecies = wizardCard.Species;
                        break;
                    }
                }
                else
                {
                    firstSpecies = Stack.Deck.First().Species;
                }
            }
            card = CheckServe(currentPlayer.OnHandCards, card, firstSpecies);
            if (card == null) {Program.IsValid = false;}
        } while (!Program.IsValid);
        return card;
    }

    private static int GetPrediction(WizardPlayer currentPlayer)
    {
        do
        {
            BuildUpInterface();
            BuildUpPlayerInterface(currentPlayer);
            if(!Program.IsValid) Program.FalseInput();
            Console.Write(Translator.Translate("Predict") + ":\t");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out var result)) continue;
            if (Program.ValidateInteger(result, new Range(0, currentPlayer.OnHandCards.Count))) return result;
        } while (!Program.IsValid);
        return 0;
    }

    private static void BuildUpPlayerInterface(WizardPlayer currentPlayer)
    {
        Program.ClearConsole(currentPlayer.Name, false);
        Console.WriteLine();
        Console.Write(Translator.Translate("On Hand") + ":\t");
        foreach (var wizardCard in currentPlayer.OnHandCards)
        {
            DisplayCard(wizardCard);
        }
        Console.WriteLine();
        Console.WriteLine();
    }

    private static void BuildUpInterface()
    {
        Program.ClearConsole(nameof(Wizard), true);
        Console.WriteLine();
        Console.Write(Translator.Translate("Trump") + ":\t");
        if(!string.IsNullOrEmpty(TrumpSpecies)) DisplayCard(new WizardCard(TrumpSpecies, -1));
        Console.WriteLine();
        Console.WriteLine();
        Console.Write(Translator.Translate("Stack") + ":\t");
        foreach (var stackCard in Stack.Deck)
        {
            DisplayCard(stackCard);
        }
        Console.WriteLine();
        Console.WriteLine();
        Console.Write(Translator.Translate("Status") + ":\t");
        var sorted = new List<WizardPlayer>(WizardPlayers.OrderBy(item => item.Id));
        foreach (var wizardPlayer in sorted)
        {
            var prediction = wizardPlayer.Prediction;
            var tricks = wizardPlayer.Tricks;
            var difference = tricks - prediction;
            if(wizardPlayer.Name.Length <= 3)
            {
                Console.Write(wizardPlayer.Name + ": ");
            }
            else
            {
                Console.Write(wizardPlayer.Name[..3] + ": ");
            }
            Console.Write(wizardPlayer.Tricks);
            var status = DisplayPredictionStatus(difference);
            Console.Write(status);
            Console.ResetColor();
            Console.Write("; ");
        }
        Console.WriteLine();
        Console.WriteLine();
        
    }

    private static string DisplayPredictionStatus(int difference)
    {
        var statusString = "";
        switch (difference)
        {
            case > 0:
                Console.ForegroundColor = ConsoleColor.Red;
                statusString += "\u207A";
                break;
            case < 0:
                Console.ForegroundColor = ConsoleColor.Red;
                statusString += "\u207B";
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
        }
        foreach (var number in Math.Abs(difference).ToString())
        {
            switch (number.ToString())
            {
                case "0":
                    statusString += "\u2070";
                    break;
                case "1":
                    statusString += "\u00B9";
                    break;
                case "2":
                    statusString += "\u00B2";
                    break;
                case "3":
                    statusString += "\u00B3";
                    break;
                case "4":
                    statusString += "\u2074";
                    break;
                case "5":
                    statusString += "\u2075";
                    break;
                case "6":
                    statusString += "\u2076";
                    break;
                case "7":
                    statusString += "\u2077";
                    break;
                case "8":
                    statusString += "\u2078";
                    break;
                case "9":
                    statusString += "\u2079";
                    break;
            }
        }
        return statusString;

    }

    private static string? ChooseTrumpSpecies(WizardPlayer player, string? trumpSpecies)
    {
        var chosenColor = ConsoleColor.Black;
        do
        {
            Program.ClearConsole(nameof(Wizard), true);
            Console.WriteLine();
            Console.Write("Trump:\t\t");
            DisplayCard(new WizardCard(trumpSpecies, 14));
            Console.WriteLine();
            Program.ClearConsole(player.Name, false);
            Console.WriteLine();
            Console.Write("Cards in Hand:\t");
            foreach (var card in player.OnHandCards)
            {
                DisplayCard(card);
                Console.Write(" ");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(Translator.Translate("Choose one of the options below."));
            foreach (var spec in WizardCardDeck.Species)
            {
                DisplayCard(new WizardCard(spec, -1));
                if (WizardCardDeck.Species.Last() != spec) Console.Write(", ");
            }
            Console.WriteLine();
            if (!Program.IsValid) Program.FalseInput();
            Console.WriteLine(Translator.Translate("Your choice: "));
            chosenColor = ValidateChosenColor(Console.ReadLine());
            if (chosenColor == ConsoleColor.Black)
            {
                Program.IsValid = false;
            }
        } while (chosenColor == ConsoleColor.Black);

        switch (chosenColor)
        {
            case ConsoleColor.Blue:
                return "Humans";
            case ConsoleColor.Green:
                return "Elves";
            case ConsoleColor.Red:
                return "Dwarfs";
            case ConsoleColor.Yellow:
                return "Giants";
            case ConsoleColor.Black:
            case ConsoleColor.DarkBlue:
            case ConsoleColor.DarkGreen:
            case ConsoleColor.DarkCyan:
            case ConsoleColor.DarkRed:
            case ConsoleColor.DarkMagenta:
            case ConsoleColor.DarkYellow:
            case ConsoleColor.Gray:
            case ConsoleColor.DarkGray:
            case ConsoleColor.Cyan:
            case ConsoleColor.Magenta:
            case ConsoleColor.White:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void DisplayCard(WizardCard wizardCard)
    {
        Console.ForegroundColor = GetConsoleColor(wizardCard.Species, wizardCard.Value);
        switch (wizardCard.Value)
        {
            case 0:
                Console.Write("J");
                break;
            case 14:
                Console.Write("W");
                break;
            case -1:
                Console.Write(wizardCard.Species);
                break;
            default:
                Console.Write(wizardCard.Species[..1] + wizardCard.Value);
                break;
        }
        Console.Write(" ");
        Console.ResetColor();
    }

    private static int GetPlayerCount()
    {
        do
        {
            Program.ClearConsole(nameof(Wizard), true);
            Console.WriteLine(Translator.Translate("How many players do you want to play with?") + "\n");
            if (!Program.IsValid) Program.FalseInput();
            Console.Write(Translator.Translate("Your choice: "));
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out var result))
            {
                Program.IsValid = false;
                continue;
            }
            if(Program.ValidateInteger(result, new Range(3, 6))) return result;
        } while (!Program.IsValid);
        return 0;
    }

    private static string[] GetPlayersNames(int playerCount)
    {
        var playersNames = new string[playerCount];
        for (var i = 0; i < playerCount; i++)
        {
            Program.ClearConsole(nameof(Wizard), true);
            Console.WriteLine($"{Translator.Translate("What is your name, player")} {i + 1}?\n");
            Console.Write(Translator.Translate("Your name: "));
            var name = Console.ReadLine();
            if (!string.IsNullOrEmpty(name)) playersNames[i] = name;
            else playersNames[i] = "Player " + (i + 1);
        }
        return playersNames;
    }
}