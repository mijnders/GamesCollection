namespace GamesCollection.Domain.Wizard;

public class Deck
{
    public List<WizardCard> Cards = new();
    public List<Player> PlayedPlayer { get; set; }

    public Deck()
    {
        
    }
}