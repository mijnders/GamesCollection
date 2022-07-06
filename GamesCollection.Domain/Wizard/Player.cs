namespace GamesCollection.Domain.Wizard;

public class Player
{
    public int PlayerNumber;
    public WizardCard[] CardsOnHand;

    public int Prediction;

    public Player(int playerNumber, WizardCard[] cardsOnHand, int prediction)
    {
        this.PlayerNumber = playerNumber;
        this.CardsOnHand = cardsOnHand;
        this.Prediction = prediction;
    }
}