namespace GamesCollection.Domain.Wizard
{
    internal class WizardCard
    {
        public ConsoleColor Color { get; set; }
        public string Species { get; set; }
        public int Value { get; set; }
        public bool WizardOrJester { get; set; }

        public WizardCard(string species, int value)
        {
            Species = species;
            Value = value;
            if (Value == 0 | Value >= 14)
            {
                WizardOrJester = true;
                Color = ConsoleColor.White;
            }
            if (WizardOrJester == false)
            {
                Color = species switch
                {
                    "Human" => ConsoleColor.Blue,
                    "Dwarf" => ConsoleColor.Red,
                    "Elves" => ConsoleColor.Green,
                    "Giant" => ConsoleColor.Yellow,
                    _ => ConsoleColor.White
                };
            }
        }
    }
}
