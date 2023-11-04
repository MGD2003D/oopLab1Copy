using Xunit;
using OOP_ICT.Models;
using System.Linq;

public class CardDeckTests
{
    [Fact]
    public void Constructor_CreatesFullDeckOfCards()
    {
        // Arrange & Act
        var deck = new CardDeck();

        // Assert
        Assert.Equal(52, deck.Cards.Count);
        Assert.Equal(52, deck.Cards.Distinct().Count());
    }

    [Fact]
    public void Shuffle_ChangesOrderOfCards()
    {
        // Arrange
        var deck = new CardDeck();
        var originalOrder = deck.Cards.ToList();

        // Act
        deck.Shuffle();
        var shuffledOrder = deck.Cards.ToList();

        // Assert
        Assert.NotEqual(originalOrder, shuffledOrder);
    }

    [Fact]
    public void DrawCard_RemovesTopCardFromDeck()
    {
        // Arrange
        var deck = new CardDeck();
        var expectedCard = deck.Cards.First();

        // Act
        var drawnCard = deck.DrawCard();

        // Assert
        Assert.Equal(expectedCard, drawnCard);
        Assert.Equal(51, deck.Cards.Count);
    }

    [Fact]
    public void DrawCard_ThrowsException_WhenDeckIsEmpty()
    {
        // Arrange
        var deck = new CardDeck();
        while (deck.Cards.Count > 0)
        {
            deck.DrawCard();
        }

        // Act & Assert
        var exception = Assert.Throws<DeckException>(() => deck.DrawCard());
        Assert.Equal("Deck is empty.", exception.Message);
    }

    [Fact]
    public void SetCards_SetsNewDeckOfCards()
    {
        // Arrange
        var deck = new CardDeck();
        var newCards = new System.Collections.Generic.List<Card>
        {
            new Card(Suit.Hearts, Rank.Ace),
            new Card(Suit.Spades, Rank.King)
        };

        // Act
        deck.SetCards(newCards);

        // Assert
        Assert.Equal(2, deck.Cards.Count);
        Assert.Contains(new Card(Suit.Hearts, Rank.Ace), deck.Cards);
        Assert.Contains(new Card(Suit.Spades, Rank.King), deck.Cards);
    }
}
