using System.ComponentModel.Design;

namespace GamesCollection.Domain.Wizard;

public abstract class StartWizard
{
    private static readonly string[] Species = { "Human", "Dwarf", "Elves", "Giant" };
    public static Deck WizardCardDeck { get; set; }
    public static List<Player> CurrentPlayers { get; set; }
    public static Deck Stack { get; set; }

    public static void Start()
    {
        Program.IsValid = true;
        do
        {
            Console.Clear();
            Console.WriteLine(Translator.Translate("How many players do you want to play with?"));
            if (!Program.IsValid) Program.FalseInput();
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out var result)) continue;
            Program.IsValid = Program.ValidateInteger(result, new Range(3, 6));
            if (!Program.IsValid) continue;
            var maxRounds = 60 / result;
            CurrentPlayers = CreatePlayers(result).ToList();
            for (var round = 5; round <= maxRounds; round++)
            {
                WizardCardDeck = CreateDeck();
                var decks = HandOutCards(result, round);
                for (var deckIndex = 0; deckIndex < decks.Count; deckIndex++)
                {
                    CurrentPlayers[deckIndex].CardsOnHand = decks[deckIndex];
                    CurrentPlayers[deckIndex].Prediction = GetPrediction(deckIndex, decks[deckIndex]);
                }
                for (var currentRound = 0; currentRound < round; currentRound++)
                {
                    ResetStack(true);
                    foreach (var currentPlayer in CurrentPlayers)
                    {
                        Console.Clear();
                        ShowStack();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine("Player " + (CurrentPlayers.IndexOf(currentPlayer) + 1));
                        Console.Write("On Hand: ");
                        ShowHandCards(currentPlayer.CardsOnHand);
                        Console.WriteLine();
                        Console.WriteLine("Which one to play");
                        PlayCards(currentPlayer);
                    }
                    CurrentPlayers[CurrentPlayers.IndexOf(Stack.PlayedPlayer[Stack.Cards.IndexOf(WhoGetTheTrick(Stack))])].Tricks++;
                }

                foreach (var player in CurrentPlayers)
                {
                    if (player.Prediction == player.Tricks)
                    {
                        player.Point += 20;
                        player.Point += player.Tricks;
                    }
                    else
                    {
                        var difference = Math.Abs(player.Tricks - player.Prediction);
                        player.Point -= difference * 10;
                    }
                }
            }
        } while (!Program.IsValid);
    }

    private static WizardCard WhoGetTheTrick(Deck stack)
    {
        var trick = "";
        for (var i = 0; i < stack.Cards.Count; i++)
        {
            if (stack.PlayedPlayer[i].PlayerNumber != 99)
            {
                if (stack.Cards[i].WizardOrJester)
                {
                    if (stack.Cards[i].Value == 14)
                    {
                        return stack.Cards[i];
                    }
                    break;
                }
                trick = stack.Cards[i].Species;
                break;
            }
            if (stack.Cards[i].WizardOrJester)
            {
                break;
            }

            trick = stack.Cards[i].Species;
            stack.Cards.Remove(stack.Cards.First());
        }
        var highestCard = stack.Cards.First();
        foreach (var stackCard in stack.Cards)
        {
            if (highestCard.Value < stackCard.Value | stack.PlayedPlayer[stack.Cards.IndexOf(highestCard)].PlayerNumber == 99)
            {
                if (stackCard.Species == trick | stackCard.Color.ToString() == trick)
                {
                    highestCard = stackCard;
                }
            }
            else if (stackCard.Value == 14 && highestCard.Value < 14)
            {
                highestCard = stackCard;
            }
        }
        return highestCard;
    }

    private static void PlayCards(Player currentPlayer)
    {
        while (true)
        {
            if (!Program.IsValid) Program.FalseInput();
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;
            var card = GetCardFromHand(input, currentPlayer.CardsOnHand);
            if (card != null)
            {
                Stack.Cards.Add(card);
                currentPlayer.CardsOnHand.Cards.Remove(card);
                Stack.PlayedPlayer.Add(currentPlayer);
                return;
            }
            Program.IsValid = false;
        }
    }

    private static WizardCard? GetCardFromHand(string input, Deck currentPlayerDeck)
    {
        if (int.TryParse(input, out var result) && result <= currentPlayerDeck.Cards.Count)
        {
            return currentPlayerDeck.Cards[result - 1];
        }
        return currentPlayerDeck.Cards.FirstOrDefault(card => input == card.Species[..1] + card.Value);
    }

    private static void ShowHandCards(Deck currentPlayersDeck)
    {
        foreach (var card in currentPlayersDeck.Cards)
        {
            DisplayCard(card);
        }
    }

    private static IEnumerable<Player> CreatePlayers(int quantity)
    {
        var players = new Player[quantity];
        for (var i = 0; i < quantity; i++)
        {
            Console.Write($"Who is the {i + 1}. player?: ");
            players[i] = new Player { Name = Console.ReadLine(), PlayerNumber = i };
        }
        return players;
    }

    private static List<Deck> HandOutCards(int quantity, int round)
    {
        var rand = new Random();
        var cards = new List<Deck>();
        for (var i = 0; i < quantity; i++)
        {
            cards.Add(new Deck());
        }
        for (var i = 0; i < round; i++)
        {
            foreach (var card in cards)
            {
                var randInt = rand.Next(WizardCardDeck.Cards.Count);
                var selectedCard = WizardCardDeck.Cards[randInt];
                card.Cards.Add(selectedCard);
                WizardCardDeck.Cards.Remove(selectedCard);
            }
        }
        ResetStack(false);
        return cards;
    }

    private static void ResetStack(bool onlyReset)
    {
        var rand = new Random();
        Stack = new Deck
        {
            Cards = new List<WizardCard>(),
            PlayedPlayer = new List<Player>()
        };
        if (WizardCardDeck.Cards.Count <= 0 && onlyReset) return;
        var card = WizardCardDeck.Cards[rand.Next(WizardCardDeck.Cards.Count)];
        Stack.Cards.Add(card);
        WizardCardDeck.Cards.Remove(card);
        Stack.PlayedPlayer.Add(new Player { Name = "Computer", PlayerNumber = 99 });
    }

    private static int GetPrediction(int playerIndex, Deck currentPlayersDeck)
    {

        while (true)
        {
            Console.Clear();
            ShowStack();
            Console.WriteLine();
            Console.Write($"Player {playerIndex} press Enter");
            Console.WriteLine();
            if (!Program.IsValid) Program.FalseInput();
            Console.ReadKey();
            Console.Clear();
            ShowStack();
            Console.WriteLine();
            Console.Write($"Player {playerIndex}:\t");
            ShowHandCards(currentPlayersDeck);
            Console.WriteLine();
            //Schleife um Stack und PlayerDeck
            if (Stack.Cards.Count == 1 && Stack.Cards.First().Value == 14) GetTrump();
            if (!Program.IsValid) Program.FalseInput();
            Console.WriteLine("Insert your prediction");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out var result))
            {
                Program.IsValid = false;
                continue;
            }
            Program.IsValid = Program.ValidateInteger(result, new Range(0, currentPlayersDeck.Cards.Count));
            if (Program.IsValid) return result;
        }
    }

    private static void GetTrump()
    {
        while (Stack.Cards.First().Color == ConsoleColor.White)
        {
            Console.WriteLine("Select on of the following Colors as Trump Color");
            foreach (var specy in Species)
            {
                DisplayCard(new WizardCard(specy, -1));
                if (Species.Last() != specy) Console.Write(", ");
            }

            Console.WriteLine();
            var colorInput = Console.ReadLine();
            if (string.IsNullOrEmpty(colorInput)) continue;
            if (Enum.TryParse(colorInput, out ConsoleColor resultColor) |
                int.TryParse(colorInput, out int colorIndex))
            {
                if (resultColor == ConsoleColor.Red | resultColor == ConsoleColor.Blue |
                    resultColor == ConsoleColor.Yellow | resultColor == ConsoleColor.Green)
                {
                    Stack.Cards.First().Color = resultColor;
                }

                if (colorIndex <= Species.Length)
                {
                    Stack.Cards.First().Color = colorIndex switch
                    {
                        1 => ConsoleColor.Blue,
                        2 => ConsoleColor.Red,
                        3 => ConsoleColor.Green,
                        4 => ConsoleColor.Yellow,
                        _ => ConsoleColor.White
                    };
                }
            }
            else
            {
                Stack.Cards.First().Color = colorInput switch
                {
                    "Human" => ConsoleColor.Blue,
                    "Dwarf" => ConsoleColor.Red,
                    "Elves" => ConsoleColor.Green,
                    "Giant" => ConsoleColor.Yellow,
                    "H" => ConsoleColor.Blue,
                    "D" => ConsoleColor.Red,
                    "E" => ConsoleColor.Green,
                    "G" => ConsoleColor.Yellow,
                    _ => ConsoleColor.White
                };
            }
        }
    }

    private static Deck CreateDeck()
    {
        var deck = new Deck();
        foreach (var spec in Species)
        {
            for (var i = 0; i <= 14; i++)
            {
                deck.Cards.Add(new WizardCard(spec, i));
            }
        }
        return deck;
    }

    private static void ShowStack()
    {
        Console.Write("stack:\t\t");
        if (Stack.Cards.Count <= 0) return;
        foreach (var stackCard in Stack.Cards)
        {
            DisplayCard(stackCard);
        }
    }

    private static void DisplayCard(WizardCard card)
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
            case -1:
                Console.Write($"{card.Color}({card.Species[..1]})");
                break;
            default:
                Console.Write(card.Species[..1] + card.Value + " ");
                break;
        }
        Console.ResetColor();
    }
}