namespace GamesCollection.Domain.Wizard;

public class WizardCardDeck
{
    public List<WizardCard> Deck { get; set; }
    public static string[] Species = { "Humans", "Giants", "Dwarfs", "Elves" };
    public WizardCardDeck()
    {
        var cards = new List<WizardCard>();
        foreach (var spec in Species)
        {
            for (var i = 0; i < 15; i++)
            {
                cards.Add(new WizardCard(spec, i));
            }
        }
        this.Deck = cards;
    }
}