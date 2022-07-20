﻿using static GamesCollection.Domain.Wizard.WizardLogic;

namespace GamesCollection.Domain.Wizard;

internal class WizardDomain
{
    public static void StartPoint()
    {
        var playerCount = GetPlayerCount();
        var wizardPlayers = CreatePlayers(playerCount, GetPlayer(playerCount));
        for (var stitchRound = 1; stitchRound <= (60 / playerCount); stitchRound++)
        {
            var mainCardDeck = CreateMainDeck();
            var wizardCardsForHands = HandsOutCards(stitchRound, playerCount, mainCardDeck);
            var firstPlayer = wizardPlayers[(stitchRound - 1) % playerCount];
            var trumpSpecies = GetTrumpSpecies(mainCardDeck);
            var stack = new WizardCardDeck
            {
                Deck = new List<WizardCard>()
            };
            var modulo = (stitchRound - 1) % playerCount;
            wizardPlayers = SortWizardPlayers(new List<WizardPlayer>(wizardPlayers.OrderBy(item => item.Id)), wizardPlayers.First(wizardPlayer => wizardPlayer.Id == modulo));
            for (var playerIndex = 0; playerIndex < playerCount; playerIndex++)
            {
                wizardPlayers[playerIndex].OnHandCards = wizardCardsForHands[playerIndex];
                if (trumpSpecies == "Wizard") trumpSpecies = ChooseTrumpSpecies(firstPlayer, trumpSpecies);
                wizardPlayers[playerIndex].Prediction = GetPrediction(wizardPlayers[playerIndex], stack, trumpSpecies, wizardPlayers);
            }
            for (var round = 0; round < stitchRound; round++)
            {
                var playedBy = new List<int>();
                stack.Deck.Clear();
                foreach (var currentPlayer in wizardPlayers)
                {
                    BuildUpInterface(trumpSpecies, stack, wizardPlayers);
                    BuildUpPlayerInterface(currentPlayer);
                    var card = currentPlayer.Com ? WizardCom.PlayCard(currentPlayer, trumpSpecies, stack, playedBy) : PlayCard(currentPlayer, stack, trumpSpecies, wizardPlayers);
                    stack.Deck.Add(card);
                    currentPlayer.OnHandCards.Remove(card);
                    playedBy.Add(currentPlayer.Id);
                    Console.ReadKey();

                }
                var indexOfPlayer = CheckPlayedCards(playedBy, stack, trumpSpecies);
                BuildUpInterface(trumpSpecies, stack, wizardPlayers);
                Console.ReadKey();
                foreach (var wizardPlayer in wizardPlayers.Where(wizardPlayer => wizardPlayer.Id == indexOfPlayer))
                {
                    wizardPlayer.Tricks++;
                }
                wizardPlayers = SortWizardPlayers(wizardPlayers, wizardPlayers.First(wizardPlayer => wizardPlayer.Id == indexOfPlayer));
            }

            foreach (var player in wizardPlayers)
            {
                CalculateExp(player);
            }
            Console.WriteLine();
            ShowIntermediate(wizardPlayers, trumpSpecies, stack);
            stack.Deck.Clear();
        }

        Console.ReadKey();
    }

    private static void ShowIntermediate(List<WizardPlayer> wizardPlayers, string trumpSpecies, WizardCardDeck stack)
    {
        BuildUpInterface(trumpSpecies, stack, new List<WizardPlayer>(wizardPlayers.OrderBy(item => item.Id)));
        Program.ClearConsole("Intermediate", false);
        Console.WriteLine();
        foreach (var player in new List<WizardPlayer>(wizardPlayers.OrderBy(item => item.Id)))
        {
            Console.Write($"{player.Name}: {player.Tricks}/{player.Prediction}");
            if (player != new List<WizardPlayer>(wizardPlayers.OrderBy(item => item.Id)).Last()) Console.Write(";\t");
            player.Tricks = 0;
            player.Prediction = 0;
        }
        Console.WriteLine();
        Console.WriteLine();
        foreach (var player in new List<WizardPlayer>(wizardPlayers.OrderBy(item => item.Id)))
        {
            Console.Write($"{player.Name}: {player.ExperiencePoints}");
            if (player != new List<WizardPlayer>(wizardPlayers.OrderBy(item => item.Id)).Last() && player.ExperiencePoints.ToString().Length > 2) Console.Write(";\t");
            else if (player != new List<WizardPlayer>(wizardPlayers.OrderBy(item => item.Id)).Last()) Console.Write(";\t\t");
        }
        Console.WriteLine();
        Console.ReadKey();
    }

    private static WizardCard PlayCard(WizardPlayer currentPlayer, WizardCardDeck stack, string trumpSpecies, List<WizardPlayer> wizardPlayers)
    {
        do
        {
            if (!Program.IsValid) Program.FalseInput();
            Console.Write(Translator.Translate("Playing") + ":\t");
            var input = Console.ReadLine();
            var card = CheckCard(currentPlayer.OnHandCards, input);
            string? firstSpecies = null;
            if (stack.Deck.Count > 0)
            {
                if (stack.Deck.First().Value is 14 or 0)
                {
                    foreach (var wizardCard in stack.Deck.Where(wizardCard => wizardCard.Value != 14 | wizardCard.Value != 0))
                    {
                        firstSpecies = wizardCard.Species;
                        break;
                    }
                }
                else
                {
                    firstSpecies = stack.Deck.First().Species;
                }
            }
            card = CheckServe(currentPlayer.OnHandCards, card, firstSpecies);
            if (card != null)
            {
                return card;
            }
            else
            {
                Program.IsValid = false;
            }
        } while (!Program.IsValid);
        return new WizardCard("Wizard", -1);
    }

    private static int GetPrediction(WizardPlayer currentPlayer, WizardCardDeck stack, string trumpSpecies, List<WizardPlayer> wizardPlayers)
    {
        do
        {
            BuildUpInterface(trumpSpecies, stack, wizardPlayers);
            BuildUpPlayerInterface(currentPlayer);
            if (!Program.IsValid) Program.FalseInput();
            Console.Write(Translator.Translate("Predict") + ":\t");
            if (!currentPlayer.Com)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) || !int.TryParse(input, out var result)) continue;
                if (Program.ValidateInteger(result, new Range(0, currentPlayer.OnHandCards.Count))) return result;
            }
            else
            {
                return WizardCom.CountPrediction(currentPlayer, trumpSpecies);
            }
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

    private static void BuildUpInterface(string trumpSpecies, WizardCardDeck stack, List<WizardPlayer> wizardPlayers)
    {
        Program.ClearConsole(nameof(Wizard), true);
        Console.WriteLine();
        Console.Write(Translator.Translate("Trump") + ":\t");
        if (!string.IsNullOrEmpty(trumpSpecies)) DisplayCard(new WizardCard(trumpSpecies, -1));
        Console.WriteLine();
        Console.WriteLine();
        Console.Write(Translator.Translate("stack") + ":\t");
        foreach (var stackCard in stack.Deck)
        {
            DisplayCard(stackCard);
        }
        Console.WriteLine();
        Console.WriteLine();
        Console.Write(Translator.Translate("Status") + ":\t");
        var sorted = new List<WizardPlayer>(wizardPlayers.OrderBy(item => item.Id));
        foreach (var wizardPlayer in sorted)
        {
            var prediction = wizardPlayer.Prediction;
            var tricks = wizardPlayer.Tricks;
            var difference = tricks - prediction;
            if (wizardPlayer.Name.Length <= 3)
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

    private static string ChooseTrumpSpecies(WizardPlayer player, string trumpSpecies)
    {
        ConsoleColor chosenColor;
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
            var input = Console.ReadLine();
            chosenColor = !string.IsNullOrEmpty(input) ? ValidateChosenColor(input) : ConsoleColor.Black;
            if (chosenColor == ConsoleColor.Black)
            {
                Program.IsValid = false;
            }
        } while (chosenColor == ConsoleColor.Black);

        return chosenColor switch
        {
            ConsoleColor.Blue => "Humans",
            ConsoleColor.Green => "Elves",
            ConsoleColor.Red => "Dwarfs",
            ConsoleColor.Yellow => "Giants",
            _ => throw new ArgumentOutOfRangeException()
        };
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
            if (Program.ValidateInteger(result, new Range(3, 6))) return result;
        } while (!Program.IsValid);
        return 0;
    }

    private static Dictionary<string, bool> GetPlayer(int playerCount)
    {
        var dict = new Dictionary<string, bool>();
        for (var i = 0; i < playerCount; i++)
        {
            var colombo = false;
            Program.ClearConsole(nameof(Wizard), true);
            Console.WriteLine($"{Translator.Translate("Should be Player")} {i + 1} {Translator.Translate("a COM")}?\n");
            Console.Write(Translator.Translate("Your choice") + " (y/n): ");
            var isCom = Console.ReadLine();
            if (!string.IsNullOrEmpty(isCom)) colombo = ValidateBool(isCom);
            if (!colombo)
            {
                Program.ClearConsole(nameof(Wizard), true);
                Console.WriteLine($"{Translator.Translate("What is your name, player")} {i + 1}?\n");
                Console.Write(Translator.Translate("Your name: "));
                var name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name))
                {
                    dict.Add(name, colombo);
                }
                else
                {
                    dict.Add("Player " + (i + 1), colombo);
                }
            }
            else
            {
                dict.Add("P" + (i + 1), colombo);
            }
        }
        return dict;
    }
}