using Newtonsoft.Json.Linq;

namespace lab_2;

public class AccountService
{
    private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "AccountList.json");
    private readonly ILogger? _logger;
    private readonly IAccountDAO _accountDAO;

    public AccountService(IAccountDAO accountDAO, ILogger? logger = null)
    {
        _accountDAO = accountDAO;
        _logger = logger;
    }

    public List<PlayerAccount> LoadAccounts()
    {
        return _accountDAO.LoadAccounts();
    }
    public void SaveAccount(PlayerAccount account)
    {
        _accountDAO.SaveAccount(account);
    }
    public virtual PlayerAccount GetAccountByCredentials(string username, string password)
    {
        var accounts = LoadAccounts();
        return accounts.FirstOrDefault(a => a.Username == username && a.Password == password);
    }

    public PlayerAccount CreateAccount(string username, string email, string password, double initialBalance)
    {
        var accountBuilder = new AccountBuilder();
        var account = accountBuilder
            .SetUsername(username)
            .SetEmail(email)
            .SetPassword(password)
            .SetInitialBalance(initialBalance)
            .Build();

        _accountDAO.SaveAccount(account);

        accountBuilder.Reset();

        return account;
    }

    public virtual JToken? FindAccountByUsername(string username)
    {
        return _accountDAO.FindAccountByUsername(username);
    }
    public virtual void UpdateAccount(PlayerAccount updatedAccount)
    {
        _accountDAO.UpdateAccount(updatedAccount);
    }

    public void UpdatePlayerScore(string username, int newScore)
    {
        _accountDAO.UpdatePlayerScore(username, newScore);
    }

    public virtual void UpdateDealerScore(string username, int newDealerScore)
    {
        _accountDAO.UpdateDealerScore(username, newDealerScore);
    }
    public virtual GameStatus GetGameStatus(string username)
    {
        var gameLogicService = new GameLogicService(_accountDAO, _logger);
        return gameLogicService.GetGameStatus(username);
    }

    public virtual void UpdateAccountBalance(string username, double betAmount, GameStatus gameResult)
    {
        var gameLogicService = new GameLogicService(_accountDAO, _logger);
        gameLogicService.UpdateAccountBalance(username, betAmount, gameResult);
    }

    public virtual void SetPlayerStopped(string username)
    {
        var account = FindAccountByUsername(username);
        if (account == null)
        {
            _logger?.Log($"SetPLayerStopped: Username not found: {username}");
            throw new InvalidOperationException($"Username not found: {username}");
        }

        try
        {
            account["isPlayerStopped"] = true;
            SaveAccount(account.ToObject<PlayerAccount>());
        }
        catch (Exception ex)
        {
            _logger?.Log($"SetPlayerStopped: Error setting status for {username}: {ex.Message}");
            throw;
        }
    }
    public virtual bool AccountExists(string username)
    {
        try
        {
            var accounts = LoadAccounts();
            return accounts.Any(a => a.Username == username);
        }
        catch (Exception ex)
        {
            _logger?.Log($"AccountExists: Account not found: {ex.Message}");
            throw;
        }
    }
    public virtual int GetPlayerScore(string username)
    {
        try
        {
            var accountJson = FindAccountByUsername(username);
            if (accountJson != null)
            {
                return accountJson["PlayerScore"].Value<int>();
            }
            else
            {
                _logger?.Log($"GetPLayerScore: Username not found: {username}");
                throw new InvalidOperationException($"Username not found: {username}");
            }
        }
        catch (Exception ex)
        {
            _logger?.Log($"Error in GetPlayerScore: {ex.Message}");
            throw;
        }
    }

    public int GetDealerScore(string username)
    {
        try
        {
            var accountJson = FindAccountByUsername(username);
            if (accountJson != null)
            {
                return accountJson["DealerScore"].Value<int>();
            }
            else
            {
                _logger?.Log($"GetDealerScore: Username not found: {username}");
                throw new InvalidOperationException($"Username not found: {username}");
            }
        }
        catch (Exception ex)
        {
            _logger?.Log($"Error in GetDealerScore: {ex.Message}");
            throw;
        }
    }

    public virtual double GetBetAmount(string username)
    {
        try
        {
            var accountJson = FindAccountByUsername(username);
            if (accountJson != null)
            {
                return accountJson["bet"].Value<double>();
            }
            else
            {
                _logger?.Log($"GetBetAmount: Username not found: {username}");
                throw new InvalidOperationException($"Username not found: {username}");
            }
        }
        catch (Exception ex)
        {
            _logger?.Log($"Error in GetBetAmount: {ex.Message}");
            throw;
        }
    }
}
