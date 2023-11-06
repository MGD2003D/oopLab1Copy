namespace OOP_ICT.Models
{
    public class Dealer
    {
        private CardDeck deck;

        public Dealer()
        {
            deck = new CardDeck();
        }

        public void PerfectShuffle()
        {
            List<Card> tempDeck = new List<Card>();
            int half = deck.Cards.Count / 2;

            for (int i = 0; i < half; i++)
            {
                tempDeck.Add(deck.Cards[i + half]);
                tempDeck.Add(deck.Cards[i]);
            }

            deck.SetCards(tempDeck);
        }

        public Card DealCard()
        {
            return deck.DrawCard();
        }

        public IReadOnlyList<Card> ShowDeck()
        {
            return deck.Cards;
        }

    }
}

