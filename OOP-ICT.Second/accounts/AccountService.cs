using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static lab_2.gameStatus;

namespace lab_2;

public class AccountService
{
    private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "AccountList.json");
    private readonly ILogger? _logger;

    public AccountService()
    {
        _logger = null;
    }

    public AccountService(ILogger logger)
    {
        _logger = logger;
    }

    public List<PlayerAccount> LoadAccounts()
    {
        try
        {
            var json = File.ReadAllText(_filePath);
            var jsonDocument = JObject.Parse(json);
            var accountsArray = jsonDocument["accounts"].ToObject<List<PlayerAccount>>();
            return accountsArray ?? new List<PlayerAccount>();
        }
        catch (Exception ex)
        {
            _logger?.Log($"Error loading accounts: {ex.Message}");
            throw;
        }
    }


    public void SaveAccount(PlayerAccount account)
    {
        var jsonData = ReadJsonFile();
        var accountsArray = jsonData["accounts"] as JArray ?? new JArray();

        var existingAccount = accountsArray.FirstOrDefault(a => a["Username"].Value<string>() == account.Username);
        if (existingAccount != null)
        {
            existingAccount["Email"] = account.Email;
            existingAccount["Password"] = account.Password;
            existingAccount["Balance"] = account.Balance;
        }
        else
        {
            var newAccountJson = JToken.FromObject(account);
            accountsArray.Add(newAccountJson);
        }

        jsonData["accounts"] = accountsArray;
        WriteJsonFile(jsonData);
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

        SaveAccount(account);

        accountBuilder.Reset();

        return account;
    }

    public virtual JObject ReadJsonFile()
    {
        if (!File.Exists(_filePath))
        {
            return new JObject(new JProperty("accounts", new JArray()));
        }

        string json = File.ReadAllText(_filePath);
        return JObject.Parse(json);
    }

    public virtual void WriteJsonFile(JObject jsonData)
    {
        try
        {
            File.WriteAllText(_filePath, jsonData.ToString(Formatting.Indented));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger?.Log($"Нет доступа к файлу: {ex.Message}");
            throw;
        }
        catch (IOException ex)
        {
            _logger?.Log($"Ошибка при записи файла: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger?.Log($"Непредвиденная ошибка при записи файла: {ex.Message}");
            throw;
        }
    }

    public virtual JToken? FindAccountByUsername(string username)
    {
        try
        {
            var jsonData = ReadJsonFile();
            var accountsArray = jsonData["accounts"] as JArray;

            if (accountsArray == null)
            {
                _logger?.Log("Формат файла не соответствует ожидаемому: отсутствует ключ 'accounts'.");
                return null;
            }

            return accountsArray.FirstOrDefault(acc => acc["Username"].Value<string>() == username);
        }
        catch (JsonReaderException ex)
        {
            _logger?.Log($"Ошибка чтения JSON: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger?.Log($"Ошибка при поиске пользователя: {ex.Message}");
            throw;
        }
    }
    public virtual void UpdateAccount(PlayerAccount updatedAccount)
    {
        var jsonData = ReadJsonFile();
        var accountsArray = jsonData["accounts"] as JArray;

        if (accountsArray != null)
        {
            var accountToUpdate = accountsArray.FirstOrDefault(a => a["Username"].Value<string>() == updatedAccount.Username);
            if (accountToUpdate != null)
            {
                accountToUpdate["Email"] = updatedAccount.Email;
                accountToUpdate["Password"] = updatedAccount.Password;
                accountToUpdate["Balance"] = updatedAccount.Balance;
                WriteJsonFile(jsonData);
            }
        }
    }

    public virtual void UpdatePlayerScore(string username, int newScore)
    {
        var jsonData = ReadJsonFile();
        var accountsArray = jsonData["accounts"] as JArray ?? new JArray();

        var account = accountsArray.FirstOrDefault(a => a["Username"].Value<string>() == username);
        if (account != null)
        {
            account["PlayerScore"] = newScore;
            WriteJsonFile(jsonData);
        }
        else
        {
            _logger?.Log($"Username not found: {username}");
        }
    }
    public virtual void UpdateDealerScore(string username, int newDealerScore)
    {
        var jsonData = ReadJsonFile();
        var accountsArray = jsonData["accounts"] as JArray ?? new JArray();

        var account = accountsArray.FirstOrDefault(a => a["Username"].Value<string>() == username);
        if (account != null)
        {
            account["DealerScore"] = newDealerScore;
            WriteJsonFile(jsonData);
        }
        else
        {
            _logger?.Log($"Username not found: {username}");
        }
    }
    public virtual GameStatus GetGameStatus(string username)
    {
        var account = FindAccountByUsername(username);
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


    public virtual void UpdateAccountBalance(string username, double betAmount, GameStatus gameResult)
    {
        var account = FindAccountByUsername(username);
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
            SaveAccount(account.ToObject<PlayerAccount>());
        }
        catch (Exception ex)
        {
            _logger?.Log($"Error updating account balance for {username}: {ex.Message}");
            throw;
        }
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
