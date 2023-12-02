using lab_2;
using lab_3;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;
using OOP_ICT.Models;
using static lab_2.gameStatus;

public class BlackjackGameTests
{
    private readonly Mock<AccountService> _mockAccountService;
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<Dealer> _mockDealer;
    private readonly BlackjackGame _blackjackGame;

    public BlackjackGameTests()
    {
        _mockAccountService = new Mock<AccountService>();
        _mockLogger = new Mock<ILogger>();
        _mockDealer = new Mock<Dealer>();
        _blackjackGame = new BlackjackGame(_mockAccountService.Object, _mockLogger.Object);
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
        var mockAccountService = new Mock<AccountService>();
        var mockLogger = new Mock<ILogger>();
        var accountJson = new JObject
        {
            ["Username"] = "testuser"
        };

        mockAccountService.Setup(s => s.FindAccountByUsername("testuser")).Returns(accountJson);
        mockAccountService.Setup(s => s.UpdateAccount(It.IsAny<PlayerAccount>())).Verifiable();

        var blackjackGame = new BlackjackGame(mockAccountService.Object, mockLogger.Object);

        // Act
        blackjackGame.StartGame("testuser", 100);

        // Assert
        Assert.Equal("blackjack", accountJson["nowplaying"].Value<string>());
        Assert.Equal(100, accountJson["bet"].Value<int>());
        Assert.True(accountJson.ContainsKey("PlayerScore"));
        Assert.True(accountJson.ContainsKey("DealerScore"));
        Assert.False(accountJson["isPlayerStopped"].Value<bool>());

        mockAccountService.Verify(s => s.UpdateAccount(It.IsAny<PlayerAccount>()), Times.Once);
        mockLogger.Verify(l => l.Log($"testuser started blackjack with a bet of 100."), Times.Once);
    }

    [Fact]
    public void StartGame_WithNonExistentUser_ShouldLogError()
    {
        // Arrange
        var mockAccountService = new Mock<AccountService>();
        var mockLogger = new Mock<ILogger>();

        mockAccountService.Setup(s => s.FindAccountByUsername("nonexistentuser")).Returns((JToken)null);

        var blackjackGame = new BlackjackGame(mockAccountService.Object, mockLogger.Object);

        // Act
        blackjackGame.StartGame("nonexistentuser", 100);

        // Assert
        mockLogger.Verify(l => l.Log($"Failed to start game for nonexistentuser: account not found."), Times.Once);
    }
    [Fact]
    public void EndGame_ResetsGameData()
    {
        // Arrange
        var mockAccountService = new Mock<AccountService>();
        var mockLogger = new Mock<ILogger>();
        var accountJson = new JObject
        {
            ["Username"] = "testuser",
            ["nowplaying"] = true,
            ["bet"] = 100,
            ["PlayerScore"] = 15,
            ["DealerScore"] = 10,
            ["isPlayerStopped"] = false
        };

        mockAccountService.Setup(s => s.FindAccountByUsername("testuser")).Returns(accountJson);
        mockAccountService.Setup(s => s.UpdateAccount(It.IsAny<PlayerAccount>())).Verifiable();

        var blackjackGame = new BlackjackGame(mockAccountService.Object, mockLogger.Object);

        // Act
        blackjackGame.EndGame("testuser");

        // Assert
        Assert.Null(accountJson["nowplaying"]);
        Assert.Null(accountJson["bet"]);
        Assert.Null(accountJson["PlayerScore"]);
        Assert.Null(accountJson["DealerScore"]);
        Assert.Null(accountJson["isPlayerStopped"]);

        mockAccountService.Verify(s => s.UpdateAccount(It.IsAny<PlayerAccount>()), Times.Once);
        mockLogger.Verify(l => l.Log($"Game ended for testuser."), Times.Once);
    }

    [Fact]
    public void EndGame_WithNonExistentUser_ShouldLogError()
    {
        // Arrange
        var mockAccountService = new Mock<AccountService>();
        var mockLogger = new Mock<ILogger>();

        mockAccountService.Setup(s => s.FindAccountByUsername("nonexistentuser")).Returns((JToken)null);

        var blackjackGame = new BlackjackGame(mockAccountService.Object, mockLogger.Object);

        // Act
        blackjackGame.EndGame("nonexistentuser");

        // Assert
        mockLogger.Verify(l => l.Log($"Attempted to end game for non-existent user: nonexistentuser."), Times.Once);
    }
    [Fact]
    public void PlayerTurn_ProcessesTurnCorrectly()
    {
        // Arrange
        string username = "testUser";
        var initialScore = 10;
        var newScore = 15;
        var mockAccount = new JObject
        {
            ["Username"] = username,
            ["PlayerScore"] = initialScore
        };

        _mockAccountService.Setup(s => s.AccountExists(username)).Returns(true);
        _mockAccountService.Setup(s => s.GetPlayerScore(username)).Returns(initialScore);

        // Act
        _blackjackGame.PlayerTurn(username);

        // Assert
        _mockAccountService.Verify(s => s.AccountExists(username), Times.Once);
        _mockAccountService.Verify(s => s.GetPlayerScore(username), Times.Once);
    }

    [Fact]
    public void DealerTurn_ProcessesTurnCorrectly()
    {
        // Arrange
        string username = "testUser";
        var mockAccount = new JObject
        {
            ["Username"] = username,
            ["DealerScore"] = 10
        };
        var card = new Card(Suit.Hearts, Rank.Five);

        _mockAccountService.Setup(s => s.AccountExists(username)).Returns(true);
        _mockAccountService.Setup(s => s.FindAccountByUsername(username)).Returns(mockAccount);
        _mockAccountService.Setup(s => s.UpdateDealerScore(username, It.IsAny<int>())).Verifiable();
        _mockDealer.Setup(d => d.DealCard()).Returns(card);

        // Act
        _blackjackGame.DealerTurn(username);

        // Assert
        _mockAccountService.Verify(s => s.UpdateDealerScore(username, It.IsAny<int>()), Times.Once);
    }



    [Fact]
    public void CheckWin_PlayerWins_ReturnsWin()
    {
        // Arrange
        string username = "testUser";
        _mockAccountService.Setup(s => s.GetGameStatus(username)).Returns(GameStatus.Win);

        // Act
        var result = _blackjackGame.CheckWin(username);

        // Assert
        Assert.Equal(GameStatus.Win, result);
    }




    [Fact]
    public void Reward_PlayerWins_IncreasesBalance()
    {
        // Arrange
        string username = "testUser";
        double betAmount = 100;

        _mockAccountService.Setup(s => s.AccountExists(username)).Returns(true);
        _mockAccountService.Setup(s => s.GetGameStatus(username)).Returns(GameStatus.Win);
        _mockAccountService.Setup(s => s.GetBetAmount(username)).Returns(betAmount);
        _mockAccountService.Setup(s => s.UpdateAccountBalance(username, betAmount, GameStatus.Win)).Verifiable();

        // Act
        _blackjackGame.Reward(username);

        // Assert
        _mockAccountService.Verify(s => s.UpdateAccountBalance(username, betAmount, GameStatus.Win), Times.Once);
    }
}