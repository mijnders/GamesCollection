﻿namespace GamesCollection.Domain.Wizard;

public class WizardCard
{
    public string Title { get; set; }
    public string Species { get; set; }
    public int Value { get; set; }

    public WizardCard(string species, int value)
    {
        Species = species;
        Value = value;
        Title = value switch
        {
            0 => "Jester",
            14 => "Wizard",
            _ => species[..1] + value
        };
    }
}