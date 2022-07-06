namespace GamesCollection.Domain
{
    public class Translator
    {
        public static List<string[]> Translation = new();
        public static int TranslationIndex = -1;

        public static void StartTranslator(List<string[]> languages)
        {
            Translation = languages;
        }
        public static string Translate(string englishText)
        {
            if (string.IsNullOrEmpty(englishText)) return englishText;
            foreach (var translate in Translation.Where(translate => translate.Contains(englishText, StringComparer.CurrentCultureIgnoreCase)))
            {
                return Translation[Translation.IndexOf(translate)][TranslationIndex];
            }
            return englishText;
        }
        public static string TranslateBackwards(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            foreach (var translate in Translation.Where(translate => translate.Contains(text, StringComparer.CurrentCultureIgnoreCase)))
            {
                return Translation[Translation.IndexOf(translate)][0];
            }
            return text;
        }

        public static string TranslateAll(string text, string language)
        {
            if (!Translation.First().Contains(language, StringComparer.CurrentCultureIgnoreCase)) return text;
            var indexOf = Translation.First().ToList().IndexOf(language);
            var indexOfText = 0;
            foreach (var translate in Translation)
            {
                if (translate.Contains(text))
                {
                    indexOfText = Translation.IndexOf(translate);
                    break;
                }
                else
                {
                    indexOfText = -1;
                }
            }

            if (indexOfText != -1 && indexOf <= Translation.Count && indexOfText <= Translation.First().Length)
            {
                return Translation[indexOfText][indexOf];
            }
            return text;
        }
    }
}
