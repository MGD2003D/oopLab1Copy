using Xunit;
using lab_2;
using Moq;

namespace lab_2_test;

public class PlayerAccountTests
{
    [Fact]
    public void Deposit_ShouldIncreaseBalance()
    {
        // Arrange
        var account = new PlayerAccount(id: 1, username: "testUser", email: "test@example.com", password: "testPassword", initialBalance: 100);
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
        var account = new PlayerAccount(id: 1, username: "testUser", email: "test@example.com", password: "testPassword", initialBalance: 100);
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
        var account = new PlayerAccount(id: 1, username: "testUser", email: "test@example.com", password: "testPassword", initialBalance: 100);
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
        var account = new PlayerAccount(id: 1, username: "testUser", email: "test@example.com", password: "testPassword", initialBalance: 100);
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
        var account = new PlayerAccount(id: 1, username: "testUser", email: "test@example.com", password: "testPassword", initialBalance: 100);
        double betAmount = 150;

        // Act
        bool canAfford = account.CanAffordBet(betAmount);

        // Assert
        Assert.False(canAfford);
    }

    [Fact]
    public void PlayerAccount_Constructor_ShouldInitializeFieldsProperly()
    {
        // Arrange
        int expectedId = 1;
        string expectedUsername = "testUser";
        string expectedEmail = "test@example.com";
        string expectedPassword = "testPassword";
        double expectedBalance = 100;

        // Act
        var account = new PlayerAccount(expectedId, expectedUsername, expectedEmail, expectedPassword, expectedBalance);

        // Assert
        Assert.Equal(expectedId, account.Id);
        Assert.Equal(expectedUsername, account.Username);
        Assert.Equal(expectedEmail, account.Email);
        Assert.Equal(expectedPassword, account.Password);
        Assert.Equal(expectedBalance, account.Balance);
    }

    [Fact]
    public void PlayerAccount_Constructor_ShouldAssignUniqueId()
    {
        // Arrange
        var account1 = new AccountBuilder().SetUsername("user1").SetEmail("email1@example.com").SetPassword("password1").SetInitialBalance(100).Build();
        var account2 = new AccountBuilder().SetUsername("user2").SetEmail("email2@example.com").SetPassword("password2").SetInitialBalance(200).Build();

        // Act
        int id1 = account1.Id;
        int id2 = account2.Id;

        // Assert
        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void AccountBuilder_Build_WithoutRequiredFields_ShouldThrow()
    {
        // Arrange
        var builder = new AccountBuilder();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Equal("Cannot create account, some required fields are not set.", exception.Message);
    }
}