namespace GamesCollection.Domain.Wizard;

public class WizardPlayer
{
    public static string[] ExampleNames = { "Peter", "Hans", "Wolfgang", "Klaus", "Manfred", "Jürgen", "Michael", "Bernd", "Werner", "Günther", "Uwe", "Dieter", "Karl", "Helmut", "Rainer", "Gerhard", "Horst", "Harald", "Joachim", "Rolf", "Holger", "Heinz", "Reinhardt", "Gerd", "Norbert", "Thomas", "Ulrich", "Herbert", "Detlev", "Josef", "Walter", "Wilfried", "Alexander", "Lothar", "Rüdiger" };
    internal int Id { get; set; }
    internal string Name { get; set; }
    internal int ExperiencePoints { get; set; }
    internal List<WizardCard?> OnHandCards { get; set; }
    internal int Prediction { get; set; }
    internal int Tricks { get; set; }
    internal bool Com { get; set; }

    public WizardPlayer(int id, string name, bool isCom, List<WizardCard?> onHandCards)
    {
        this.Id = id;
        this.Name = name;
        Com = isCom;
        OnHandCards = onHandCards;
    }
}