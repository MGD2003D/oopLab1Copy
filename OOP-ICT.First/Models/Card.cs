namespace OOP_ICT.Models;

public class Card
{
    public Suit CardSuit { get; }
    public Rank CardRank { get; }

    public Card(Suit suit, Rank rank)
    {
        CardSuit = suit;
        CardRank = rank;
    }

    public override string ToString()
    {
        return $"{CardRank} of {CardSuit}";
    }

    public override bool Equals(object obj)
    {
        if (obj is Card other)
        {
            return CardSuit == other.CardSuit && CardRank == other.CardRank;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CardSuit, CardRank);
    }
}
