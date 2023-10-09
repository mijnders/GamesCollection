namespace GamesCollection.Domain;

public static class Translator
{
    private static List<string[]> _translation = new();
    public static int TranslationIndex = -1;

    public static void StartTranslator(List<string[]> languages)
    {
        _translation = languages;
    }
    public static string Translate(string englishText)
    {
        if (string.IsNullOrEmpty(englishText)) return englishText;
        foreach (var translate in _translation.Where(translate => translate.Contains(englishText, StringComparer.CurrentCultureIgnoreCase)))
        {
            return _translation[_translation.IndexOf(translate)][TranslationIndex];
        }
        return englishText;
    }
    public static string TranslateBackwards(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        foreach (var translate in _translation.Where(translate => translate.Contains(text, StringComparer.CurrentCultureIgnoreCase)))
        {
            return _translation[_translation.IndexOf(translate)][0];
        }
        return text;
    }

    public static string TranslateAll(string text, string language)
    {
        if (!_translation.First().Contains(language, StringComparer.CurrentCultureIgnoreCase)) return text;
        var indexOf = _translation.First().ToList().IndexOf(language);
        var indexOfText = 0;
        foreach (var translate in _translation)
        {
            if (translate.Contains(text))
            {
                indexOfText = _translation.IndexOf(translate);
                break;
            }

            indexOfText = -1;
        }

        if (indexOfText != -1 && indexOf <= _translation.Count && indexOfText <= _translation.First().Length)
        {
            return _translation[indexOfText][indexOf];
        }
        return text;
    }
}