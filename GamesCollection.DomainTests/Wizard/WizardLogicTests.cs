using Microsoft.VisualStudio.TestTools.UnitTesting;
using GamesCollection.Domain.Wizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesCollection.Domain.Wizard.Tests
{
    [TestClass()]
    public class WizardLogicTests
    {
        List<int> _playedBy = new()
        {
            0, 1
        };

        private WizardCardDeck _stack = new WizardCardDeck()
        {
            Deck = new List<WizardCard>()
        };

        [TestMethod()]
        public void CheckPlayedCardsTest()
        {

            Assert.Fail();
        }
    }
}