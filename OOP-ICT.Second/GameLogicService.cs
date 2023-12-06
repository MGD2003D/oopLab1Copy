using Newtonsoft.Json.Linq;

namespace lab_2;

public class GameLogicService
{
    private readonly IAccountDAO _accountDAO;
    private readonly ILogger? _logger;

    public GameLogicService(IAccountDAO accountDAO, ILogger? logger)
    {
        _accountDAO = accountDAO;
        _logger = logger;
    }

    public GameStatus GetGameStatus(string username)
    {
        var account = _accountDAO.FindAccountByUsername(username);
        if (account == null)
        {
            _logger?.Log($"GetGameStatus: Username not found: {username}");
            throw new InvalidOperationException($"Username not found: {username}");
        }

        try
        {
            int playerScore = account["PlayerScore"].Value<int>();
            int dealerScore = account["DealerScore"].Value<int>();
            bool isPlayerStopped = account["isPlayerStopped"].Value<bool>();

            if (playerScore > 21) return GameStatus.Lose;
            if (dealerScore > 21) return GameStatus.Win;
            if (playerScore == 21) return GameStatus.Win;
            if (dealerScore == 21) return GameStatus.Lose;
            if (isPlayerStopped)
            {
                if (playerScore > dealerScore) return GameStatus.Win;
                else if (playerScore < dealerScore) return GameStatus.Lose;
                else return GameStatus.Draw;
            }
            if (playerScore < 21 && dealerScore < 21) return GameStatus.InProgress;
        }
        catch (Exception ex)
        {
            _logger?.Log($"Error getting game status for {username}: {ex.Message}");
            throw;
        }

        throw new InvalidOperationException($"Unexpected condition in GetGameStatus for username: {username}");
    }

    public void UpdateAccountBalance(string username, double betAmount, GameStatus gameResult)
    {
        var account = _accountDAO.FindAccountByUsername(username);
        if (account == null)
        {
            _logger?.Log($"UpdateAccountBalance: Username not found: {username}");
            throw new InvalidOperationException($"Username not found: {username}");
        }

        try
        {
            double balance = account["Balance"].Value<double>();

            switch (gameResult)
            {
                case GameStatus.Win:
                    balance += betAmount;
                    break;
                case GameStatus.Lose:
                    balance -= betAmount;
                    break;
                default:
                    throw new ArgumentException($"Invalid game result: {gameResult}");
            }

            account["Balance"] = balance;
            _accountDAO.SaveAccount(account.ToObject<PlayerAccount>());
        }
        catch (Exception ex)
        {
            _logger?.Log($"Error updating account balance for {username}: {ex.Message}");
            throw;
        }
    }
}
