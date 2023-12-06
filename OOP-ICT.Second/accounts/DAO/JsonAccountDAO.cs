using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace lab_2;

public class JsonAccountDAO : IAccountDAO
{
    private readonly string _filePath;
    private readonly ILogger? _logger;

    public JsonAccountDAO(string filePath, ILogger? logger)
    {
        _filePath = filePath;
        _logger = logger;
    }

    protected virtual JObject ReadJsonFile()
    {
        if (!File.Exists(_filePath))
        {
            return new JObject(new JProperty("accounts", new JArray()));
        }

        string json = File.ReadAllText(_filePath);
        return JObject.Parse(json);
    }

    protected virtual void WriteJsonFile(JObject jsonData)
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

    public JToken? FindAccountByUsername(string username)
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

    public void UpdateAccount(PlayerAccount updatedAccount)
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

    public void UpdatePlayerScore(string username, int newScore)
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

    public void UpdateDealerScore(string username, int newDealerScore)
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
}
