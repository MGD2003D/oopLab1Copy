using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_ICT.Models;




public class CardDeck
{
    private List<Card> cards;
    public IReadOnlyList<Card> Cards => cards.AsReadOnly();
    public void SetCards(List<Card> newCards)
    {
        cards = newCards;
    }

    public CardDeck()
    {
        cards = Enum.GetValues(typeof(Suit))
            .Cast<Suit>()
            .SelectMany(suit => Enum.GetValues(typeof(Rank)).Cast<Rank>(),
                        (suit, rank) => new Card(suit, rank))
            .ToList();
    }


    public IEnumerable<Rank> GetOrderedRanks()
    {
        return Enum.GetValues(typeof(Rank)).Cast<Rank>();
    }


    public void Shuffle()
    {
        Random rng = new Random();
        cards = cards.OrderBy(x => rng.Next()).ToList();
    }

    public Card DrawCard()
    {
        if (cards.Count == 0) throw new DeckException("Deck is empty.");

        var drawnCard = cards[0];
        cards.RemoveAt(0);
        return drawnCard;
    }


    public int Count => cards.Count;

    public override string ToString()
    {
        return string.Join(", ", cards);
    }
}