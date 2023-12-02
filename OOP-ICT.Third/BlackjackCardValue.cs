using OOP_ICT.Models;

namespace lab_3;

public static class BlackjackCardValue
{
    private const int ValueOfFaceCard = 10;
    private const int ValueOfAce = 11;
    private const int BlackjackLimit = 21;
    private const int AceHighToLowDifference = 10;

    public static int CalculateCardValue(Card card)
    {
        switch (card.CardRank)
        {
            case Rank.Jack:
            case Rank.Queen:
            case Rank.King:
                return ValueOfFaceCard;
            case Rank.Ace:
                return ValueOfAce;
            default:
                return (int)card.CardRank;
        }
    }

    public static int AdjustForAces(List<Card> hand, int currentScore)
    {
        int aceCount = hand.Count(card => card.CardRank == Rank.Ace);
        while (currentScore > BlackjackLimit && aceCount > 0)
        {
            currentScore -= AceHighToLowDifference;
            aceCount--;
        }
        return currentScore;
    }
}