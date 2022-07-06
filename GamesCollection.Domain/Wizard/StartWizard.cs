using System.Dynamic;

namespace GamesCollection.Domain.Wizard;

public abstract class StartWizard
{
    private static readonly string[] Species = {"Human", "Dwarf", "Elves", "Giant"};
    public static Deck WizardCardDeck { get; set; }

    public static List<Player> CurrentPlayers { get; set; }

    public static void Start()
    {
        Program.IsValid = true;
        WizardCardDeck = CreateDeck();
        do
        {
            Console.WriteLine(Translator.Translate("How many players do you want to play with?"));
            if(!Program.IsValid) Program.FalseInput();
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out var result)) continue;
            Program.IsValid = Program.ValidateInteger(result, new Range(3, 6));
            if (Program.IsValid) CurrentPlayers = CreatePlayers(result).ToList();
        } while (!Program.IsValid);
    }

    private static Player[] CreatePlayers(int quantity)
    {
        var players = new Player[quantity];
        for (int i = 0; i < quantity; i++)
        {
            players[i] = new Player();
        }
    }

    private static Deck CreateDeck()
    {
        var deck = new Deck();
        foreach (var spec in Species)
        {
            for (var i = 0; i < 14; i++)
            {
                deck.Cards.Add(new WizardCard(spec, i));
            }
        }
        return deck;
    }
}