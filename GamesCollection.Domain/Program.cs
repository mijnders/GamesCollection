using GamesCollection.Domain.RockPaperScissor;
using GamesCollection.Domain.Wizard;

namespace GamesCollection.Domain
{
    public class Program
    {
        private static void Main()
        {
            Console.ResetColor();
            LoadFiles.LoadOnStartup();
            var languages = LoadFiles.LoadLanguages();
            Translator.StartTranslator(languages);
            ChoseLanguage(languages);
            var games = Sort(LoadFiles.LoadGames());
            var running = true;
            var falseInput = false;
            do
            {
                Console.Clear();
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

                if (falseInput)
                {
                    FalseInput();
                    falseInput = false;
                }

                var input = Console.ReadLine();
                if (int.TryParse(input, out var result) && input != "0" && result < games.Count)
                {
                    StartGame(Translator.TranslateBackwards(games[result]));
                }
                else if (input != null && Translator.TranslateBackwards(input).ToLower() == "exit" | input.ToLower() == "x" | input.ToLower() == "close" | input == "0")
                {
                    running = false;
                }
                else if (input != null && games.FindIndex(item => string.Equals(item, input, StringComparison.CurrentCultureIgnoreCase)) != -1)
                {
                    StartGame(Translator.TranslateBackwards(input));
                }
                else
                {
                    falseInput = true;
                }
            } while (running);

            Console.WriteLine(Translator.Translate("Press Enter to continue"));
            Console.ReadKey();
            Console.Clear();
        }

        private static void ChoseLanguage(List<string[]> languages)
        {
            var falseInput = false;
            while (true)
            {
                Console.Clear();
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
                if (falseInput) FalseInput();
                Console.Write("Your choice: ");
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) falseInput = true;
                else if (int.TryParse(input, out var result) && result <= languages.First().Length)
                {
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
                else falseInput = true;
            }

        }

        public static void FalseInput()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Translator.TranslationIndex == -1
                ? "Your last entry was invalid. Please try again"
                : Translator.Translate("Your last entry was invalid. Please try again"));
            Console.ResetColor();
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

        public static void StartGame(string game)
        {
            Console.Clear();
            Console.ResetColor();
            var width = (Console.WindowWidth - Translator.Translate(game).Length) / 2;
            for (var i = 0; i < width; i++) Console.Write("-");
            Console.Write(Translator.Translate(game));
            for (var i = 0; i < width; i++) Console.Write("-");
            Console.WriteLine();
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
}