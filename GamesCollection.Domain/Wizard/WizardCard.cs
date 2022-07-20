namespace GamesCollection.Domain.Wizard;

public class WizardCard
{
    internal string Title { get; set; }
    internal string Species { get; set; }
    internal int Value { get; set; }

    public WizardCard(string species, int value)
    {
        Species = species;
        this.Value = value;
        Title = value switch
        {
            0 => "J",
            14 => "Wizard",
            _ => species[..1] + value
        };
    }
}