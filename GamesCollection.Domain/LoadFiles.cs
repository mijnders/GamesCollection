namespace GamesCollection.Domain;

public static class LoadFiles
{
    private static string _gameFile = "";
    private static string _languagesFile = "";
    public static void LoadOnStartup()
    {
        var files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));
        foreach (var file in files)
        {
            if (Path.GetFileNameWithoutExtension(file) == "Games")
            {
                _gameFile = file;
            }
            if (Path.GetFileNameWithoutExtension(file) == "Languages")
            {
                _languagesFile = file;
            }
        }
    }

    public static List<string> LoadGames()
    {
        return File.ReadAllLines(_gameFile).Select(Translator.Translate).ToList();
    }
    public static List<string[]> LoadLanguages()
    {
        if (File.Exists(_languagesFile))
        {
            while (true)
            {
                try
                {
                    return File.ReadAllLines(_languagesFile).Select(s => s.Split(";")).ToList();
                }
                catch (IOException)
                {
                    Console.WriteLine("Please close the file and try again");
                    Console.ReadKey();
                }
            }
        }

        return new List<string[]>();

    }

}