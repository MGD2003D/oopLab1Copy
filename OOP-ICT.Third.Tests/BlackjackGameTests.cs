using lab_2;
using lab_3;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

public class BlackjackGameTests
{
    private readonly Mock<AccountService> _mockAccountService;
    private readonly BlackjackGame _blackjackGame;

    public BlackjackGameTests()
    {
        _mockAccountService = new Mock<AccountService>();
        _blackjackGame = new BlackjackGame(_mockAccountService.Object);
    }

    [Fact]
    public void PlayerConnect_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        string username = "testUser";
        string password = "testPassword";
        var mockAccount = new PlayerAccount(1, username, "testuser@example.com", password, 1000.0);

        _mockAccountService.Setup(s => s.GetAccountByCredentials(username, password))
            .Returns(mockAccount);

        // Act
        var result = _blackjackGame.PlayerConnect(username, password);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void PlayerConnect_InvalidCredentials_ReturnsFalse()
    {
        // Arrange
        string username = "nonExistingUser";
        string password = "wrongPassword";

        _mockAccountService.Setup(s => s.GetAccountByCredentials(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((PlayerAccount)null);

        // Act
        var result = _blackjackGame.PlayerConnect(username, password);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public void StartGame_InitializesGameWithValidUserAndBet()
    {
        // Arrange
        string username = "testUser";
        int betAmount = 100;
        var mockAccountData = new JObject(
            new JProperty("accounts",
                new JArray(
                    new JObject(
                        new JProperty("Username", username),
                        new JProperty("Balance", 1000)
                    )
                )
            )
        );

        _mockAccountService.Setup(s => s.ReadJsonFile())
            .Returns(mockAccountData);
        _mockAccountService.Setup(s => s.WriteJsonFile(It.IsAny<JObject>()))
            .Callback<JObject>(json => mockAccountData = json);

        // Act
        _blackjackGame.StartGame(username, betAmount);

        // Assert
        var account = mockAccountData["accounts"].FirstOrDefault(acc => acc["Username"].Value<string>() == username);
        Assert.NotNull(account);
        Assert.Equal("blackjack", account["nowplaying"].Value<string>());
        Assert.Equal(betAmount, account["bet"].Value<int>());
        Assert.True(account["PlayerScore"].Value<int>() > 0);
        Assert.True(account["DealerScore"].Value<int>() > 0);
        Assert.False(account["isPlayerStopped"].Value<bool>());
    }
    [Fact]
    public void EndGame_ResetsGameData()
    {
        // Arrange
        string username = "testUser";
        JObject mockAccountData = new JObject(
            new JProperty("accounts",
                new JArray(
                    new JObject(
                        new JProperty("Username", username),
                        new JProperty("nowplaying", "blackjack")
                    )
                )
            )
        );

        _mockAccountService.Setup(s => s.ReadJsonFile()).Returns(mockAccountData);
        _mockAccountService.Setup(s => s.WriteJsonFile(It.IsAny<JObject>()))
            .Callback<JObject>(json => mockAccountData = json);

        // Act
        _blackjackGame.EndGame(username);

        // Assert
        var updatedAccount = mockAccountData["accounts"].FirstOrDefault(acc => acc["Username"].Value<string>() == username);
        Assert.NotNull(updatedAccount);
        Assert.Null(updatedAccount["nowplaying"]);
    }
    [Fact]
    public void PlayerTurn_ProcessesTurnCorrectly()
    {
        // Arrange
        string username = "testUser";
        JObject mockAccountData = new JObject(
            new JProperty("accounts",
                new JArray(
                    new JObject(
                        new JProperty("Username", username),
                        new JProperty("PlayerScore", 10)
                    )
                )
            )
        );

        _mockAccountService.Setup(s => s.ReadJsonFile()).Returns(mockAccountData);
        _mockAccountService.Setup(s => s.WriteJsonFile(It.IsAny<JObject>()))
            .Callback<JObject>(json => mockAccountData = json);

        // Act
        _blackjackGame.PlayerTurn(username);

        // Assert
        var updatedAccount = mockAccountData["accounts"].FirstOrDefault(acc => acc["Username"].Value<string>() == username);
        Assert.NotNull(updatedAccount);
        int updatedPlayerScore = updatedAccount["PlayerScore"].Value<int>();
        Assert.True(updatedPlayerScore > 10);
    }

    [Fact]
    public void DealerTurn_ProcessesTurnCorrectly()
    {
        // Arrange
        string username = "testUser";
        JObject mockAccountData = new JObject(
            new JProperty("accounts",
                new JArray(
                    new JObject(
                        new JProperty("Username", username),
                        new JProperty("DealerScore", 10)
                    )
                )
            )
        );

        _mockAccountService.Setup(s => s.ReadJsonFile()).Returns(mockAccountData);
        _mockAccountService.Setup(s => s.WriteJsonFile(It.IsAny<JObject>()))
            .Callback<JObject>(json => mockAccountData = json);

        _blackjackGame.DealerTurn(username);

        // Assert
        var updatedAccount = mockAccountData["accounts"].FirstOrDefault(acc => acc["Username"].Value<string>() == username);
        Assert.NotNull(updatedAccount);
        int updatedDealerScore = updatedAccount["DealerScore"].Value<int>();
        Assert.True(updatedDealerScore > 10);
    }


    [Fact]
    public void CheckWin_PlayerWins_ReturnsWin()
    {
        // Arrange
        string username = "testUser";
        JObject mockAccountData = new JObject(
            new JProperty("accounts",
                new JArray(
                    new JObject(
                        new JProperty("Username", username),
                        new JProperty("PlayerScore", 21),
                        new JProperty("DealerScore", 20),
                        new JProperty("isPlayerStopped", false)
                    )
                )
            )
        );

        _mockAccountService.Setup(s => s.ReadJsonFile()).Returns(mockAccountData);

        // Act
        string result = _blackjackGame.CheckWin(username);

        // Assert
        Assert.Equal("win", result);
    }

    [Fact]
    public void Reward_PlayerWins_IncreasesBalance()
    {
        // Arrange
        string username = "testUser";
        int betAmount = 100;
        double initialBalance = 1000.0;
        JObject mockAccountData = new JObject(
            new JProperty("accounts",
                new JArray(
                    new JObject(
                        new JProperty("Username", username),
                        new JProperty("Balance", initialBalance),
                        new JProperty("bet", betAmount),
                        new JProperty("PlayerScore", 21),
                        new JProperty("DealerScore", 20),
                        new JProperty("isPlayerStopped", true)
                    )
                )
            )
        );

        _mockAccountService.Setup(s => s.ReadJsonFile()).Returns(mockAccountData);
        _mockAccountService.Setup(s => s.WriteJsonFile(It.IsAny<JObject>()))
            .Callback<JObject>(json => mockAccountData = json);

        // Act
        _blackjackGame.Reward(username);

        // Assert
        var updatedAccount = mockAccountData["accounts"].FirstOrDefault(acc => acc["Username"].Value<string>() == username);
        Assert.NotNull(updatedAccount);
        double updatedBalance = updatedAccount["Balance"].Value<double>();
        Assert.Equal(initialBalance + betAmount, updatedBalance);
    }
    [Fact]
    public void PlayerStopped_SetsPlayerStoppedStatus()
    {
        // Arrange
        string username = "testUser";
        JObject mockAccountData = new JObject(
            new JProperty("accounts",
                new JArray(
                    new JObject(
                        new JProperty("Username", username),
                        new JProperty("isPlayerStopped", false)
                    )
                )
            )
        );

        _mockAccountService.Setup(s => s.ReadJsonFile()).Returns(mockAccountData);
        _mockAccountService.Setup(s => s.WriteJsonFile(It.IsAny<JObject>()))
            .Callback<JObject>(json => mockAccountData = json);

        // Act
        _blackjackGame.PlayerStopped(username);

        // Assert
        var updatedAccount = mockAccountData["accounts"].FirstOrDefault(acc => acc["Username"].Value<string>() == username);
        Assert.NotNull(updatedAccount);
        Assert.True(updatedAccount["isPlayerStopped"].Value<bool>());
    }

}






