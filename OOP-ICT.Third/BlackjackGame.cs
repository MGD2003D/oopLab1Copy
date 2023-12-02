using lab_2;
using OOP_ICT.Models;
using static lab_2.gameStatus;

namespace lab_3;

public class BlackjackGame
{
    private readonly AccountService _accountService;
    private readonly ILogger _logger;

    public BlackjackGame(AccountService accountService, ILogger logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    public bool PlayerConnect(string username, string password)
    {
        var account = _accountService.GetAccountByCredentials(username, password);
        if (account != null)
        {
            _logger.Log($"{username} connected to the game.");
            return true;
        }
        else
        {
            _logger.Log("Invalid credentials.");
            return false;
        }
    }

    public void StartGame(string username, int betAmount)
    {
        var dealer = new Dealer();
        dealer.PerfectShuffle();

        var playerHand = new List<Card> { dealer.DealCard(), dealer.DealCard() };
        var dealerHand = new List<Card> { dealer.DealCard() };

        int playerScore = playerHand.Sum(card => BlackjackCardValue.CalculateCardValue(card));
        int dealerScore = dealerHand.Sum(card => BlackjackCardValue.CalculateCardValue(card));
        playerScore = BlackjackCardValue.AdjustForAces(playerHand, playerScore);
        dealerScore = BlackjackCardValue.AdjustForAces(dealerHand, dealerScore);

        var account = _accountService.FindAccountByUsername(username);

        if (account != null)
        {
            account["nowplaying"] = "blackjack";
            account["bet"] = betAmount;
            account["PlayerScore"] = playerScore;
            account["DealerScore"] = dealerScore;
            account["isPlayerStopped"] = false;

            var updatedAccount = account.ToObject<PlayerAccount>();
            _accountService.UpdateAccount(updatedAccount);

            _logger.Log($"{username} started blackjack with a bet of {betAmount}.");
        }
        else
        {
            _logger.Log($"Failed to start game for {username}: account not found.");
        }
    }


    public void EndGame(string username)
    {
        var account = _accountService.FindAccountByUsername(username);

        if (account != null)
        {
            var propertiesToRemove = new List<string> { "nowplaying", "bet", "PlayerScore", "DealerScore", "isPlayerStopped" };
            foreach (var prop in propertiesToRemove)
            {
                account[prop].Parent.Remove();
            }

            var updatedAccount = account.ToObject<PlayerAccount>();
            _accountService.UpdateAccount(updatedAccount);

            _logger.Log($"Game ended for {username}.");
        }
        else
        {
            _logger.Log($"Attempted to end game for non-existent user: {username}.");
        }
    }

    public void PlayerTurn(string username)
    {
        if (!_accountService.AccountExists(username))
        {
            _logger.Log($"PlayerTurn: Invalid username: {username}");
            return;
        }

        var dealer = new Dealer();
        dealer.PerfectShuffle();
        var card = dealer.DealCard();

        var currentScore = _accountService.GetPlayerScore(username);

        var newScore = currentScore + BlackjackCardValue.CalculateCardValue(card);
        newScore = BlackjackCardValue.AdjustForAces(new List<Card> { card }, newScore);

        _accountService.UpdatePlayerScore(username, newScore);
    }
    public void DealerTurn(string username)
    {
        if (!_accountService.AccountExists(username))
        {
            _logger.Log($"DealerTurn: Account not found for username: {username}");
            return;
        }

        var dealer = new Dealer();
        dealer.PerfectShuffle();
        var card = dealer.DealCard();

        var dealerScore = _accountService.GetDealerScore(username);

        dealerScore += BlackjackCardValue.CalculateCardValue(card);
        dealerScore = BlackjackCardValue.AdjustForAces(new List<Card> { card }, dealerScore);

        _accountService.UpdateDealerScore(username, dealerScore);
    }

    public GameStatus CheckWin(string username)
    {
        return _accountService.GetGameStatus(username);
    }





    public void Reward(string username)
    {
        var gameResult = CheckWin(username);

        if (!_accountService.AccountExists(username))
        {
            _logger.Log($"Reward: Account not found for username: {username}");
            return;
        }

        double betAmount = _accountService.GetBetAmount(username);

        _accountService.UpdateAccountBalance(username, betAmount, gameResult);
    }
}