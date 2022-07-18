namespace GamesCollection.Domain.RockPaperScissor;

internal class BaseGame
{
    private static readonly string?[] Shapes = { "rock", "paper", "scissors" };

    private static readonly string[] Rules = { "Scissors cuts Paper", "Paper covers Rock", "(and as it always has) Rock crushes Scissors", };

    public static void Startup()
    {
        var inputInvalid = false;
        while (true)
        {
            var chosenShapes = new string[2];
            Console.Write(Translator.Translate("Do you want to play against the "));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(Translator.Translate("COM") + "(c)");
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
                        Console.Clear();
                        WhenReset();
                        chosenShapes[0] = PlayerInput(1);
                        Console.Clear();
                        WhenReset();
                        chosenShapes[1] = PlayerInput(2);
                        Console.Clear();
                        WhenReset();
                        ShowWinner(CheckWinner(chosenShapes, true), true);
                        break;
                    case "computer":
                    case "c":
                        Console.Clear();
                        WhenReset();
                        chosenShapes[0] = PlayerInput(1);
                        Console.Clear();
                        WhenReset();
                        chosenShapes[1] = ComputerInput();
                        ShowWinner(CheckWinner(chosenShapes, false), false);
                        break;
                    case "rules":
                    case "r":
                        Console.WriteLine();
                        ShowRules();
                        break;
                    case "exit":
                    case "x":
                        Console.Clear();
                        WhenReset();
                        return;
                    default:
                        Console.Clear();
                        WhenReset();
                        inputInvalid = true;
                        break;
                }
            Console.WriteLine();
            Console.WriteLine(Translator.Translate("Press Enter to continue"));
            Console.ReadKey();
            Console.Clear();
            WhenReset();
        }
    }

    private static string PlayerInput(int player)
    {
        var existingShape = true;
        var color = ConsoleColor.White;
        while (true)
        {
            color = player switch
            {
                1 => ConsoleColor.Green,
                2 => ConsoleColor.Magenta,
                _ => color
            };

            Console.ForegroundColor = color;
            Console.WriteLine($"{Translator.Translate("Player")} {player}");
            Console.ResetColor();
            Console.Write(Translator.Translate("Choose between Rock, Scissors or Paper."));
            Console.WriteLine();
            if (!existingShape) InputInvalid();
            Console.Write(Translator.Translate("Your choice: "));
            var chosenShape = Console.ReadLine() ?? throw new InvalidOperationException();
            chosenShape = Translator.TranslateBackwards(chosenShape);
            if (Shapes.Contains(chosenShape.Replace(" ", "").ToLower()))
            {
                return chosenShape.ToLower().Replace(" ", "");
            }
            Console.Clear();
            WhenReset();
            existingShape = false;
        }
    }
    private static int CheckWinner(string[] chosenShapes, bool vsPlayer)
    {
        var player1 = Translator.Translate(chosenShapes[0]);
        var player2 = Translator.Translate(chosenShapes[1]);
        Console.WriteLine(
            Translator.Translate("Player 1 played: ") + player1[..1].ToUpper() + player1[1..]);
        if (vsPlayer)
        {
            Console.WriteLine(Translator.Translate("Player 2 played: ") + player2[..1].ToUpper() + player2[1..] +
                              "\n");
        }
        else
        {
            Console.WriteLine(Translator.Translate("COM played: ") + player2[..1].ToUpper() + player2[1..] + "\n");
        }
        switch (string.Join(",", chosenShapes))
        {
            case "scissors,paper":
            case "paper,scissors":
                Console.WriteLine(Translator.Translate(Rules[0]));
                return chosenShapes.ToList().IndexOf("scissors");
            case "paper,rock":
            case "rock,paper":
                Console.WriteLine(Translator.Translate(Rules[1]));
                return chosenShapes.ToList().IndexOf("paper");
            case "rock,scissors":
            case "scissors,rock":
                Console.WriteLine(Translator.Translate(Rules[2]));
                return chosenShapes.ToList().IndexOf("rock");
            default:
                return -1;

        }
    }
    private static void ShowWinner(int winner, bool vsPlayer)
    {
        switch (winner)
        {
            case 0:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Translator.Translate("Player 1 wins!"));
                break;
            case 1 when vsPlayer:
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(Translator.Translate("Player 2 wins!"));
                break;
            case 1 when !vsPlayer:
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(Translator.Translate("COM wins!"));
                break;
            case -1:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Translator.Translate("Draw!"));
                break;
        }

        Console.ResetColor();
    }

    private static string ComputerInput()
    {
        var rnd = new Random();
        var randomNumber = rnd.Next(Shapes.Length);
        return Shapes[randomNumber] ?? throw new InvalidOperationException();
    }

    private static void ShowRules()
    {
        foreach (var rule in Rules)
        {
            Console.WriteLine("-" + Translator.Translate(rule));
        }
    }

    private static void InputInvalid()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(Translator.Translate("Your last entry was invalid. Please try again"));
        Console.ResetColor();
    }

    private static void WhenReset()
    {
        Console.ResetColor();
        var width = (Console.WindowWidth - Translator.Translate("Rock, Paper, Scissors").Length) / 2;
        for (var i = 0; i < width; i++) Console.Write("-");
        Console.Write(Translator.Translate("Rock, Paper, Scissors"));
        for (var i = 0; i < width; i++) Console.Write("-");
        Console.WriteLine();
    }
}