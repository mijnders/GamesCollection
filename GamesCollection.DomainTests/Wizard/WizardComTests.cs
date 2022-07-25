using GamesCollection.Domain.Wizard;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace GamesCollection.DomainTests.Wizard
{
    [TestClass()]
    public class WizardComTests
    {
        [TestMethod()]
        public void ChanceTestNoTrump()
        {
            //Arrange
            var result = "";
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WizardChance");
            var mainDeck = new WizardCardDeck();
            var trump = "Jester";
            var chances = new List<double>();
            var cards = new List<WizardCard>();
            //Act
            foreach (var wizardCard in mainDeck.Deck)
            {
                chances.Add(WizardCom.GetChance(wizardCard, trump, mainDeck));
                cards.Add(wizardCard);
            }

            for (var i = 0; i < cards.Count; i++)
            {
                result += $"{cards[i].Title}\t=\t{chances[i]}\n";
            }
            //Assert
            File.WriteAllText(path + "\\NoTrump.txt", result);
        }

        [TestMethod()]
        public void ChanceTestGreenTrump()
        {
            //Arrange
            var result = "";
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WizardChance");
            var mainDeck = new WizardCardDeck();
            var trump = "Elves";
            var chances = new List<double>();
            var cards = new List<WizardCard>();
            //Act
            foreach (var wizardCard in mainDeck.Deck)
            {
                chances.Add(WizardCom.GetChance(wizardCard, trump, mainDeck));
                cards.Add(wizardCard);
            }

            for (var i = 0; i < cards.Count; i++)
            {
                result += $"{cards[i].Title}\t=\t{chances[i]}\n";
            }
            //Assert
            File.WriteAllText(path + "\\GreenTrump.txt", result);
        }
        [TestMethod()]
        public void ChanceTestRedTrump()
        {
            //Arrange
            var result = "";
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WizardChance");
            var mainDeck = new WizardCardDeck();
            var trump = "Dwarfs";
            var chances = new List<double>();
            var cards = new List<WizardCard>();
            //Act
            foreach (var wizardCard in mainDeck.Deck)
            {
                chances.Add(WizardCom.GetChance(wizardCard, trump, mainDeck));
                cards.Add(wizardCard);
            }

            for (var i = 0; i < cards.Count; i++)
            {
                result += $"{cards[i].Title}\t=\t{chances[i]}\n";
            }
            //Assert
            File.WriteAllText(path + "\\RedTrump.txt", result);
        }
        [TestMethod()]
        public void ChanceTestYellowTrump()
        {
            //Arrange
            var result = "";
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WizardChance");
            var mainDeck = new WizardCardDeck();
            var trump = "Giants";
            var chances = new List<double>();
            var cards = new List<WizardCard>();
            //Act
            foreach (var wizardCard in mainDeck.Deck)
            {
                chances.Add(WizardCom.GetChance(wizardCard, trump, mainDeck));
                cards.Add(wizardCard);
            }

            for (var i = 0; i < cards.Count; i++)
            {
                result += $"{cards[i].Title}\t=\t{chances[i]}\n";
            }
            //Assert
            File.WriteAllText(path + "\\YellowTrump.txt", result);
        }
        [TestMethod()]
        public void ChanceTestBlueTrump()
        {
            //Arrange
            var result = "";
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WizardChance");
            var mainDeck = new WizardCardDeck();
            var trump = "Humans";
            var chances = new List<double>();
            var cards = new List<WizardCard>();
            //Act
            foreach (var wizardCard in mainDeck.Deck)
            {
                chances.Add(WizardCom.GetChance(wizardCard, trump, mainDeck));
                cards.Add(wizardCard);
            }

            for (var i = 0; i < cards.Count; i++)
            {
                result += $"{cards[i].Title}\t=\t{chances[i]}\n";
            }
            //Assert
            File.WriteAllText(path + "\\BlueTrump.txt", result);
        }
    }
}