using Xunit;
using OOP_ICT.Models;

public class CardTests
{
    [Fact]
    public void Constructor_SetsSuitAndRank()
    {
        // Arrange
        var suit = Suit.Hearts;
        var rank = Rank.Ace;

        // Act
        var card = new Card(suit, rank);

        // Assert
        Assert.Equal(suit, card.CardSuit);
        Assert.Equal(rank, card.CardRank);
    }

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var suit = Suit.Hearts;
        var rank = Rank.Ace;
        var card = new Card(suit, rank);
        var expectedString = "Ace of Hearts";

        // Act
        var cardString = card.ToString();

        // Assert
        Assert.Equal(expectedString, cardString);
    }
}
