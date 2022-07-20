namespace GamesCollection.Domain.Wizard
{
    public class WizardCom
    {
        public static int CountPrediction(List<WizardCard> onHandCards, string trump)
        {
            var prediction = 0;
            var copyMainDeck = new WizardCardDeck();
            foreach (var card in onHandCards)
            {
                copyMainDeck.Deck.Remove(copyMainDeck.Deck.First(item => item.Value == card.Value && item.Species == card.Species));
            }
            foreach (var card in onHandCards)
            {
                switch (card.Value)
                {
                    case 14:
                        prediction++;
                        break;
                    case 0:
                        break;
                    default:
                        if (GetChance(card, trump, copyMainDeck) <= 33)
                        {
                            prediction++;
                        }

                        break;
                }
            }
            return prediction;
        }

        public static double GetChance(WizardCard card, string trump, WizardCardDeck copyMainDeck)
        {
            var fakePlayedBy = new List<int>
            {
                0,
                1
            };
            var strongerCards = (from deckCard in copyMainDeck.Deck where card != null let fakeStack = new WizardCardDeck() { Deck = new List<WizardCard>() { card, deckCard } } where WizardLogic.CheckPlayedCards(fakePlayedBy, fakeStack, trump) != 0 select deckCard).ToList();

            return Math.Round(double.Parse(strongerCards.Count.ToString()) / double.Parse(copyMainDeck.Deck.Count.ToString()) * 100);
        }

        public static WizardCard PlayCard(WizardPlayer player, string trump, WizardCardDeck stack, List<int> playedBy)
        {
            if (player.OnHandCards.Count == 1)
            {
                return player.OnHandCards.First();
            }

            var speciesToServe = GetSpeciesToServe(stack);
            var playOffensive = player.Tricks < player.Prediction;
            if (speciesToServe == "all")
                return playOffensive ? GetHighestCard(player.OnHandCards) : GetLowestCard(player.OnHandCards);
            var listList = new List<List<WizardCard>>
            {
                player.OnHandCards.Where(card => card.Species == speciesToServe && card.Value != 14 && card.Value != 0)
                    .ToList(),
                player.OnHandCards.Where(card => card.Species == trump && card.Value != 14 && card.Value != 0).ToList(),
                player.OnHandCards.Where(card => card is {Value: 14 or 0}).ToList(),
                player.OnHandCards
            };
            foreach (var list in listList.Where(list => list.Any()))
            {
                list.Sort((x, y) => x.Value.CompareTo(y.Value));
                if (!playOffensive) list.Reverse();
                var firstCard = list.First();
                if (list.Count == 1 && firstCard.Species == speciesToServe && firstCard.Value != 14 && firstCard.Value != 0 && !listList[2].Any()) return list.First();
                foreach (var playerOnHandCard in list)
                {
                    stack.Deck.Add(playerOnHandCard);
                    playedBy.Add(player.Id);
                    if (playOffensive)
                    {
                        if (WizardLogic.CheckPlayedCards(playedBy, stack, trump) == player.Id)
                        {
                            stack.Deck.Remove(playerOnHandCard);
                            playedBy.Remove(player.Id);
                            return playerOnHandCard;
                        }

                        if (listList[2].Any(card => card is { Value: 14 }))
                            return player.OnHandCards.First(card => card is { Value: 14 });
                    }
                    else
                    {
                        if (WizardLogic.CheckPlayedCards(playedBy, stack, trump) != player.Id)
                        {
                            stack.Deck.Remove(playerOnHandCard);
                            playedBy.Remove(player.Id);
                            return playerOnHandCard;
                        }

                        if (listList[2].Any(card => card is { Value: 0 }))
                            return player.OnHandCards.First(card => card is { Value: 0 });
                    }

                    stack.Deck.Remove(playerOnHandCard);

                    playedBy.Remove(player.Id);
                }

                return GetLowestCard(list);
            }

            return playOffensive ? GetHighestCard(player.OnHandCards) : GetLowestCard(player.OnHandCards);
        }

        private static string GetSpeciesToServe(WizardCardDeck stack)
        {
            foreach (var card in stack.Deck)
            {
                if (card.Value != 14 && card.Value != 0)
                {
                    return card.Species;
                }
                else if (card.Value == 14)
                {
                    return "all";
                }
            }

            return "all";
        }

        private static WizardCard GetLowestCard(IReadOnlyCollection<WizardCard> cards)
        {
            var lowestCard = cards.First();
            if (cards.Count <= 1) return lowestCard;
            foreach (var card in cards.Where(card => lowestCard != null && card != null && card.Value < lowestCard.Value))
            {
                lowestCard = card;
            }

            return lowestCard;
        }

        private static WizardCard GetHighestCard(IReadOnlyCollection<WizardCard> cards)
        {
            var lowestCard = cards.First();
            if (cards.Count <= 1) return lowestCard;
            foreach (var card in cards.Where(card => lowestCard != null && card != null && card.Value > lowestCard.Value))
            {
                lowestCard = card;
            }

            return lowestCard;
        }

        public static ConsoleColor ChooseTrumpColor(WizardPlayer player)
        {
            var highestCounter = WizardCardDeck.Species.Select(spec => player.OnHandCards.Count(item => item.Value != 14 && item.Value != 0 && item.Species == spec)).Prepend(0).Max();
            return (from spec in WizardCardDeck.Species where player.OnHandCards.Count(item => item.Value != 14 && item.Value != 0 && item.Species == spec) == highestCounter select WizardLogic.ValidateChosenColor(spec)).FirstOrDefault();
        }
    }

}
