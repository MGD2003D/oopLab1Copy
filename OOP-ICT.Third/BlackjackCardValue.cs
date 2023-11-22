using OOP_ICT.Models;

namespace lab_3;

public static class BlackjackCardValue
{
    public static int CalculateCardValue(Card card)
    {
        switch (card.CardRank)
        {
            case Rank.Jack:
            case Rank.Queen:
            case Rank.King:
                return 10;
            case Rank.Ace:
                return 11;
            default:
                return (int)card.CardRank;
        }
    }

    public static int AdjustForAces(List<Card> hand, int currentScore)
    {
        int aceCount = hand.Count(card => card.CardRank == Rank.Ace);
        while (currentScore > 21 && aceCount > 0)
        {
            currentScore -= 10;
            aceCount--;
        }
        return currentScore;
    }
}


