﻿namespace GamesCollection.Domain.Wizard;

public class WizardLogic
{
    public static List<WizardPlayer> CreatePlayers(int playerCount, Dictionary<string, bool> names)
    {
        var playerList = new List<WizardPlayer>();
        var i = 0;
        foreach(var name in names)
        {
            playerList.Add(new WizardPlayer(i, name.Key, name.Value, new List<WizardCard>()));
            i++;
        }
        return playerList;
    }
    public static bool ValidateBool(string input)
    {
        if (bool.TryParse(input, out bool result))
        {
            return result;
        }
        switch (input.ToLower())
        {
            case "true":
            case "yes":
            case "y":
            case "1":
                return true;
            case "false":
            case "no":
            case "n":
            case "0":
                return false;
        }
        return false;
    }

    public static void CalculateExp(WizardPlayer player)
    {
        if (player.Prediction == player.Tricks)
        {
            player.ExperiencePoints += 20;
            player.ExperiencePoints += player.Tricks * 10;
        }
        else
        {
            var difference = Math.Abs(player.Prediction - player.Tricks);
            player.ExperiencePoints -= difference * 10;
        }
    }

    public static List<WizardPlayer> SortWizardPlayers(List<WizardPlayer> wizardPlayers, WizardPlayer firstplayer)
    {
        var indexOfFirst = wizardPlayers.ToList().IndexOf(firstplayer);
        for (var i = 0; i < indexOfFirst; i++)
        {
            wizardPlayers.Add(wizardPlayers.First());
            wizardPlayers.RemoveAt(0);
        }
        return wizardPlayers;
    }

    public static int CheckPlayedCards(List<int> playedBy, WizardCardDeck stack, string? trumpSpecies)
    {
        var highestCard = stack.Deck[0];
        foreach (var card in stack.Deck.Where(card => card.Species != null && highestCard.Species == null))
        {
            highestCard = card;
        }
        foreach (var card in stack.Deck.Where(card => card.Value == 14))
        {
            return playedBy[stack.Deck.IndexOf(card)];
        }
        var trumpList = stack.Deck.Where(card => card.Species == trumpSpecies && card.Value != 14 && card.Value != 0)
            .ToList();
        if (trumpList.Count > 0)
        {
            highestCard = trumpList[0];
            foreach (var card in trumpList.Where(card => highestCard.Value < card.Value))
            {
                highestCard = card;
            }
            return playedBy[stack.Deck.IndexOf(highestCard)];
        }
        var firstColorCardsList = stack.Deck.Where(card => card.Species == highestCard.Species);
        foreach (var wizardCard in firstColorCardsList)
        {
            if (wizardCard.Value > highestCard.Value)
            {
                highestCard = wizardCard;
            }
        }
        return playedBy[stack.Deck.IndexOf(highestCard)];
    }

    public static WizardCard CheckCard(List<WizardCard> onHandCards, string? input)
    {
        if (string.IsNullOrEmpty(input) && onHandCards.Count > 1) return null;
        if (string.IsNullOrEmpty(input) && onHandCards.Count == 1) return onHandCards.First();
        foreach (var wizardCard in onHandCards.Where(wizardCard => wizardCard.Title == input | input == wizardCard.Title[..1]))
        {
            return wizardCard;
        }
        if (int.TryParse(input, out var result) && result <= onHandCards.Count && result > 0)
        {
            return onHandCards[result - 1];
        }
        return null;
    }

    public static WizardCard? CheckServe(List<WizardCard> onHandCards, WizardCard? card, string? species)
    {
        if (card != null && string.IsNullOrEmpty(species) | card.Value is 14 or 0) return card;
        if (card != null && card.Species == species)
        {
            return card;
        }
        return onHandCards.Any(item => item.Species == species & item.Value < 14 & item.Value > 0) ? null : card;
    }
    public static ConsoleColor ValidateChosenColor(string input)
    {
        if (Enum.TryParse(input, out ConsoleColor resultColor) |
            int.TryParse(input, out var colorIndex))
        {
            if (resultColor == ConsoleColor.Red | resultColor == ConsoleColor.Blue |
                resultColor == ConsoleColor.Yellow | resultColor == ConsoleColor.Green)
            {
                return resultColor;
            }

            if (colorIndex <= WizardCardDeck.Species.Length)
            {
                return colorIndex switch
                {
                    1 => ConsoleColor.Blue,
                    2 => ConsoleColor.Red,
                    3 => ConsoleColor.Green,
                    4 => ConsoleColor.Yellow,
                    _ => ConsoleColor.Black
                };
            }
        }
        else
        {
            return input switch
            {
                "Humans" => ConsoleColor.Blue,
                "Dwarfs" => ConsoleColor.Red,
                "Elves" => ConsoleColor.Green,
                "Giants" => ConsoleColor.Yellow,
                "H" => ConsoleColor.Blue,
                "D" => ConsoleColor.Red,
                "E" => ConsoleColor.Green,
                "G" => ConsoleColor.Yellow,
                _ => ConsoleColor.Black
            };
        }
        return ConsoleColor.Black;
    }

    public static WizardCardDeck CreateMainDeck()
    {
        return new WizardCardDeck();
    }

    public static string GetTrumpSpecies(WizardCardDeck mainCardDeck)
    {
        if (mainCardDeck.Deck.Count <= 0) return "failed";
        var trumpCard = HandsOutCards(1, 1, mainCardDeck);
        return trumpCard.First().First().Value switch
        {
            14 => trumpCard.First().First().Title,
            0 => trumpCard.First().First().Title,
            _ => trumpCard.First().First().Species
        };
    }

    public static ConsoleColor GetConsoleColor(string? species, int value)
    {
        return value switch
        {
            14 => ConsoleColor.White,
            0 => ConsoleColor.White,
            _ => CheckForSpecies(species)
        };
    }

    public static ConsoleColor CheckForSpecies(string? species)
    {
        return species switch
        {
            "Humans" => ConsoleColor.Blue,
            "Dwarfs" => ConsoleColor.Red,
            "Giants" => ConsoleColor.Yellow,
            "Elves" => ConsoleColor.Green,
            _ => ConsoleColor.Black,
        };
    }
    public static List<WizardCard>[] HandsOutCards(int round, int playerCount, WizardCardDeck mainCardDeck)
    {
        var rand = new Random();
        var cards = new List<WizardCard>[playerCount];
        for (var playerIndex = 0; playerIndex < playerCount; playerIndex++)
        {
            cards[playerIndex] = new List<WizardCard>();
        }
        for (var roundIndex = 0; roundIndex < round; roundIndex++)
        {
            foreach (var card in cards)
            {
                var randInt = rand.Next(mainCardDeck.Deck.Count);
                var selectedCard = mainCardDeck.Deck[randInt];
                card.Add(selectedCard);
                mainCardDeck.Deck.Remove(selectedCard);
            }
        }
        return cards;
    }
}