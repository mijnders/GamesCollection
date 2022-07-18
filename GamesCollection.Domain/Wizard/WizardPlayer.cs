namespace GamesCollection.Domain.Wizard;

public class WizardPlayer
{
    internal int Id { get; set; }
    internal string Name { get; set; }
    internal int ExperiencePoints { get; set; }
    internal List<WizardCard> OnHandCards { get; set; }
    internal int Prediction { get; set; }
    internal int Tricks { get; set; }
    internal bool COM { get; set; }

    public WizardPlayer(int id, string name)
    {
        this.Id = id;
        this.Name = name;
        COM = false;
    }
}