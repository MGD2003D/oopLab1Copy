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
}
