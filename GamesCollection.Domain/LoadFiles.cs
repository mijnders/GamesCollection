using System.Collections.Generic;

namespace GamesCollection.Domain
{
    public class LoadFiles
    {
        public static string GameFile = "";
        public static string LanguagesFile = "";
        public static void LoadOnStartup()
        {
            var files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));
            foreach (var file in files)
            {
                if (Path.GetFileNameWithoutExtension(file) == "Games")
                {
                    GameFile = file;
                }
                if (Path.GetFileNameWithoutExtension(file) == "Languages")
                {
                    LanguagesFile = file;
                }
            }
        }

        public static List<string> LoadGames()
        {
            return File.ReadAllLines(GameFile).Select(Translator.Translate).ToList();
        }
        public static List<string[]> LoadLanguages()
        {
            if (File.Exists(LanguagesFile))
            {
                while (true)
                {
                    try
                    {
                        return File.ReadAllLines(LanguagesFile).Select(s => s.Split(";")).ToList();
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Please close the file and try again");
                        Console.ReadKey();
                    }
                }
            }
            else
            {
                return new List<string[]>();
            }
            
        }

    }
}
