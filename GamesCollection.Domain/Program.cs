using GamesCollection.Domain.RockPaperScissor;
using GamesCollection.Domain.Wizard;

namespace GamesCollection.Domain;

public class Program
{

    public static bool IsValid { get; set; }
    private static void Main()
    {
        IsValid = true;
        Console.ResetColor();
        LoadFiles.LoadOnStartup();
        var languages = LoadFiles.LoadLanguages();
        Translator.StartTranslator(languages);
        ChoseLanguage(languages);
        var games = Sort(LoadFiles.LoadGames());
        do
        {
            Console.Clear();
            ClearConsole("Main menu");
            Console.WriteLine(Translator.Translate("Welcome to the game collection, please choose one of the options below."));
            Console.WriteLine();
            if (games.Count == 0)
            {
                Console.WriteLine(Translator.Translate("No games were found"));
                break;
            }
            for (var i = 0; i < games.Count; i++)
            {
                Console.WriteLine($"{i}. {games[i]}");
            }

            if (!IsValid)
            {
                FalseInput();
            }
            var input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input) && int.TryParse(input, out var result))
            {
                IsValid = ValidateInteger(result, new Range(games.IndexOf(games.First()), games.IndexOf(games.Last())));
                if(IsValid)StartGame(Translator.TranslateBackwards(games[result]));
            }
            else if (!string.IsNullOrEmpty(input) && Translator.TranslateBackwards(input).ToLower() == "exit" | input.ToLower() == "x" | input.ToLower() == "close" | input == "0")
            {
                break;
            }
            else if (!string.IsNullOrEmpty(input) && games.FindIndex(item => string.Equals(item, input, StringComparison.CurrentCultureIgnoreCase)) != -1)
            {
                StartGame(Translator.TranslateBackwards(input));
            }
            else
            {
                IsValid = false;
            }
        } while (true);
        Console.WriteLine(Translator.Translate("Press Enter to continue"));
        Console.ReadKey();
        Console.Clear();
    }

    private static void ChoseLanguage(List<string[]> languages)
    {
        while (true)
        {
            Console.Clear();
            ClearConsole("Languages");
            var availableLanguages = languages.First().ToList();
            foreach (var availableLanguage in availableLanguages)
            {
                if (availableLanguages.Last() != availableLanguage) Console.Write(Translator.TranslateAll("Select your language", availableLanguage) + " / ");
                else Console.WriteLine(Translator.TranslateAll("Select your language", availableLanguage));
            }
            Console.WriteLine();
            var i = 0;
            foreach (var availableLanguage in availableLanguages)
            {
                i++;
                Console.WriteLine($"{i}. {availableLanguage}");
            }
            Console.WriteLine();
            if (!IsValid) FalseInput();
            Console.Write(Translator.Translate("Your choice:"));
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) IsValid = false;
            else if (int.TryParse(input, out var result))
            {
                IsValid = ValidateInteger(result, new Range(0, languages.First().Length));
                if (!IsValid) continue;
                Translator.TranslationIndex = result - 1;
                return;
            }
            else if (languages.First().ToList().FindIndex(item =>
                         string.Equals(item, input, StringComparison.CurrentCultureIgnoreCase)) != -1)
            {
                Translator.TranslationIndex = languages.First().ToList().FindIndex(item =>
                    string.Equals(item, input, StringComparison.CurrentCultureIgnoreCase));
                return;
            }
            else IsValid = false;
        }

    }

    public static void FalseInput()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(Translator.TranslationIndex == -1
            ? "Your last entry was invalid. Please try again"
            : Translator.Translate("Your last entry was invalid. Please try again"));
        Console.ResetColor();
        IsValid = true;
    }

    public static List<string> Sort(List<string> games)
    {
        games.Sort();
        var exit = Translator.Translate("Exit");
        var newGamesList = new List<string> { exit };
        games.Remove(exit);
        newGamesList.AddRange(games);
        return newGamesList;
    }

    public static void ClearConsole(string name)
    {
        Console.Clear();
        var width = (Console.WindowWidth - Translator.Translate(name).Length) / 2;
        for (var i = 0; i < width; i++) Console.Write("-");
        Console.Write(Translator.Translate(name));
        for (var i = 0; i < width; i++) Console.Write("-");
        Console.WriteLine();
    }

    public static bool ValidateInteger(int result, Range range)
        
    {
        return result >= range.Start.Value && result <= range.End.Value;
    }

    public static void StartGame(string game)
    {
        switch (game)
        {
            case "Mau-Mau":
                break;
            case "Uno":
                break;
            case "Wizard":
                StartWizard.Start();
                break;
            case "Rock, Paper, Scissors":
                BaseGame.Startup();
                break;
            case "Rock, Paper, Scissors, Lizard, Spock":
                LizardSpockExtension.Startup();
                break;
        }
    }
}