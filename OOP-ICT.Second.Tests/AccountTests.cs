using Xunit;
using lab_2;
using Moq;

namespace lab_2_test
{
    public class PlayerAccountTests
    {
        [Fact]
        public void Deposit_ShouldIncreaseBalance()
        {
            // Arrange
            var account = new PlayerAccount(100);
            double depositAmount = 50;

            // Act
            account.Deposit(depositAmount);

            // Assert
            Assert.Equal(150, account.Balance);
        }

        [Fact]
        public void Withdraw_WhenBalanceIsSufficient_ShouldDecreaseReturnTrue()
        {
            // Arrange
            var account = new PlayerAccount(100);
            double withdrawalAmount = 50;

            // Act
            bool result = account.Withdraw(withdrawalAmount);

            // Assert
            Assert.True(result);
            Assert.Equal(50, account.Balance);
        }

        [Fact]
        public void Withdraw_WhenBalanceIsInsufficient_ShouldNotChangeBalanceAndReturnFalse()
        {
            // Arrange
            var account = new PlayerAccount(100);
            double withdrawalAmount = 150;

            // Act
            bool result = account.Withdraw(withdrawalAmount);

            // Assert
            Assert.False(result);
            Assert.Equal(100, account.Balance);
        }

        [Fact]
        public void CanAffordBet_WhenBalanceIsSufficient_ShouldReturnTrue()
        {
            // Arrange
            var account = new PlayerAccount(100);
            double betAmount = 50;

            // Act
            bool canAfford = account.CanAffordBet(betAmount);

            // Assert
            Assert.True(canAfford);
        }

        [Fact]
        public void CanAffordBet_WhenBalanceIsInsufficient_ShouldReturnFalse()
        {
            // Arrange
            var account = new PlayerAccount(100);
            double betAmount = 150;

            // Act
            bool canAfford = account.CanAffordBet(betAmount);

            // Assert
            Assert.False(canAfford);
        }
    }
}
