using GamesCollection.Domain.Wizard;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GamesCollection.DomainTests.Wizard
{
    [TestClass()]
    public class WizardLogicTests
    {
        List<int> _playedBy = new()
        {
            0, 1
        };

        private WizardCardDeck _stack = new WizardCardDeck();
        [TestMethod()]
        public void CheckPlayedCardsOnlyJester()
        {
            var stack = new WizardCardDeck()
            {
                Deck = new List<WizardCard>()
                {
                    new WizardCard("Dwarfs", 0),
                    new WizardCard("Giants", 0),
                    new WizardCard("Humans", 0),
                    new WizardCard("Elves", 0)
                }
            };
            var index = WizardLogic.CheckPlayedCards(_playedBy, stack, "Humans");
            Assert.AreEqual(0, index);
        }
        [TestMethod()]
        public void CheckPlayedCardsOnlyWizard()
        {
            var stack = new WizardCardDeck()
            {
                Deck = new List<WizardCard>()
                {
                    new WizardCard("Dwarfs", 14),
                    new WizardCard("Giants", 14),
                    new WizardCard("Humans", 14),
                    new WizardCard("Elves", 14)
                }
            };
            var index = WizardLogic.CheckPlayedCards(_playedBy, stack, "Humans");
            Assert.AreEqual(0, index);
        }
    }
}