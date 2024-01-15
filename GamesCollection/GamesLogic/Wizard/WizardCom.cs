using System.Runtime.InteropServices.ComTypes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace GamesCollection.Domain.Wizard
{
    public static class WizardCom
    {
        public static int CountPrediction(WizardPlayer player, string trump, int stitchRound, int indexOfPlayer, int playerCount)
        {
            var prediction = 0;
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"WizardChance\\{player.Name}\\{player.Name}_{player.Id}_{stitchRound}.txt");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
            }
            var onHandCards = player.OnHandCards;
            var copyMainDeck = new WizardCardDeck();
            foreach (var card in onHandCards)
            {
                copyMainDeck.Deck.Remove(copyMainDeck.Deck.First(item => card != null && item != null && item.Value == card.Value && item.Species == card.Species));
            }
            foreach (var card in onHandCards)
            {
                var chance = GetChance(card, trump, copyMainDeck);
                if (stitchRound > playerCount)
                {
                    if (card?.Species == trump && chance <= 15)
                    {
                        prediction++;
                        File.AppendAllText(path, card.Title + " = " + chance + "%\n");
                    }
                    else if (card?.Species != trump && chance <= 33.3)
                    {
                        prediction++;
                        File.AppendAllText(path, card.Title + " = " + chance + "%\n");
                    }
                }
                else
                {
                    if (card?.Species == trump && chance <= 33.3)
                    {
                        prediction++;
                        File.AppendAllText(path, card.Title + " = " + chance + "%\n");
                    }
                    else if (card?.Species != trump && chance <= 15)
                    {
                        prediction++;
                        File.AppendAllText(path, card.Title + " = " + chance + "%\n");
                    }
                }
               
            }
            if (prediction - stitchRound / playerCount < 2) return prediction;
            var rand = new Random();
            return rand.Next(stitchRound / playerCount, prediction);
        }

        public static double GetChance(WizardCard? card, string trump, WizardCardDeck copyMainDeck)
        {
            var fakePlayedBy = new List<int>
            {
                0,
                1
            };
            var strongerCards = new List<WizardCard?>();
            foreach (var mdCard in copyMainDeck.Deck)
            {
                var fakeStack = new WizardCardDeck()
                {
                    Deck = new List<WizardCard?>()
                    {
                        card,
                        mdCard
                    }
                };
                if (WizardLogic.CheckPlayedCards(fakePlayedBy, fakeStack, trump) != 0)
                {
                    strongerCards.Add(mdCard);
                }
            }

            return Math.Round(double.Parse(strongerCards.Count.ToString()) / double.Parse(copyMainDeck.Deck.Count.ToString()) * 100, 1);
        }

        public static WizardCard? PlayCard(WizardPlayer player, string trump, WizardCardDeck stack, List<int> playedBy)
        {
            if (player.OnHandCards.Count == 1)
            {
                return player.OnHandCards.First();
            }

            var speciesToServe = GetSpeciesToServe(stack);
            var playOffensive = player.Tricks < player.Prediction;
            if (speciesToServe == "all")
            {
                var any = stack.Deck.Any(card => card.Value == 14);

                return playOffensive && !any ? GetHighestCard(player.OnHandCards, trump) : GetLowestCard(player.OnHandCards, trump);
            }

            var listList = new List<List<WizardCard?>>
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

                return GetLowestCard(list, trump);
            }

            return playOffensive ? GetHighestCard(player.OnHandCards, trump) : GetLowestCard(player.OnHandCards, trump);
        }

        private static string GetSpeciesToServe(WizardCardDeck stack)
        {
            foreach (var card in stack.Deck)
            {
                if (card.Value != 14 && card.Value != 0)
                {
                    return card.Species;
                }

                if (card.Value == 14)
                {
                    return "all";
                }
            }

            return "all";
        }

        private static WizardCard? GetLowestCard(IReadOnlyCollection<WizardCard?> cards, string trump)
        {
            var lowestCard = cards.First();
            if (cards.Count <= 1) return lowestCard;
            foreach (var card in cards.Where(card => card.Species != trump && card.Value < lowestCard.Value))
            {
                lowestCard = card;
            }

            return lowestCard;
        }

        private static WizardCard? GetHighestCard(IReadOnlyCollection<WizardCard?> cards, string trump)
        {
            var highestCard = cards.First();
            if (cards.Count <= 1) return highestCard;
            foreach (var card in cards.Where(card => card.Species != trump && card.Value > highestCard.Value))
            {
                highestCard = card;
            }

            return highestCard;
        }

        public static ConsoleColor ChooseTrumpColor(WizardPlayer player)
        {
            var highestCounter = WizardCardDeck.Species.Select(spec => player.OnHandCards.Count(item => item.Value != 14 && item.Value != 0 && item.Species == spec)).Prepend(0).Max();
            return (from spec in WizardCardDeck.Species where player.OnHandCards.Count(item => item.Value != 14 && item.Value != 0 && item.Species == spec) == highestCounter select WizardLogic.ValidateChosenColor(spec)).FirstOrDefault();
        }
    }

}
