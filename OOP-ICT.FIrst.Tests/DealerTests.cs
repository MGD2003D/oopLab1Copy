using Xunit;
using OOP_ICT.Models;
using System.Linq;
using System.Collections.Generic;

public class DealerTests
{
    [Fact]
    public void Constructor_CreatesNewDeck()
    {
        // Arrange & Act
        var dealer = new Dealer();

        // Assert
        Assert.NotNull(dealer.ShowDeck());
        Assert.Equal(52, dealer.ShowDeck().Count);
    }

    [Fact]
    public void PerfectShuffle_PerformsCorrectShuffle()
    {
        // Arrange
        var dealer = new Dealer();
        var originalDeck = new List<Card>(dealer.ShowDeck());

        // Act
        dealer.PerfectShuffle();
        var shuffledDeck = dealer.ShowDeck();

        // Assert
        Assert.NotEqual(originalDeck, shuffledDeck);

        for (int i = 0; i < shuffledDeck.Count / 2; i++)
        {
            Assert.Equal(originalDeck[i + shuffledDeck.Count / 2], shuffledDeck[2 * i]);
            Assert.Equal(originalDeck[i], shuffledDeck[2 * i + 1]);
        }
    }

    [Fact]
    public void DealCard_DealsTopCardCorrectly()
    {
        // Arrange
        var dealer = new Dealer();
        var expectedCard = dealer.ShowDeck().First();

        // Act
        var dealtCard = dealer.DealCard();

        // Assert
        Assert.Equal(expectedCard, dealtCard);
        Assert.Equal(51, dealer.ShowDeck().Count);
    }

    [Fact]
    public void DealCard_ThrowsException_WhenNoCardsLeft()
    {
        // Arrange
        var dealer = new Dealer();
        while (dealer.ShowDeck().Count > 0)
        {
            dealer.DealCard();
        }

        // Act & Assert
        Assert.Throws<DeckException>(() => dealer.DealCard());
    }
}
