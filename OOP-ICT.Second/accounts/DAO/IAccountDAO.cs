using Newtonsoft.Json.Linq;

namespace lab_2;

public interface IAccountDAO
{
    List<PlayerAccount> LoadAccounts();
    void SaveAccount(PlayerAccount account);
    JToken? FindAccountByUsername(string username);
    void UpdateAccount(PlayerAccount updatedAccount);
    void UpdatePlayerScore(string username, int newScore);
    void UpdateDealerScore(string username, int newDealerScore);
}

