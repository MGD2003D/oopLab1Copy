using Xunit;
using lab_2;
using Moq;

    public class BlackjackCasinoTests
    {
        [Fact]
        public void AwardWin_ShouldDoublTheBet()
        {
            // Arrange
            var mockAccount = new Mock<IAccount>();
            var casino = new BlackjackCasino();
            double betAmount = 100;
            double expectedWin = betAmount * 2;

            mockAccount.Setup(a => a.Deposit(expectedWin));

            // Act
            casino.AwardWin(mockAccount.Object, betAmount);

            // Assert
            mockAccount.Verify(a => a.Deposit(expectedWin), Times.Once());
        }

        [Fact]
        public void HandleBlackjack_ShouldPayOneAndHalfTimesTheBet()
        {
            // Arrange
            var mockAccount = new Mock<IAccount>();
            var casino = new BlackjackCasino();
            double betAmount = 100;
            double expectedBlackjackWin = betAmount * 1.5 + betAmount;

            mockAccount.Setup(a => a.Deposit(expectedBlackjackWin));

            // Act
            casino.HandleBlackjack(mockAccount.Object, betAmount);

            // Assert
            mockAccount.Verify(a => a.Deposit(expectedBlackjackWin), Times.Once());
        }
    }

