using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lab_2
{
    public class AccountService
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "OOP-ICT.Second", "accounts", "AccountList.json");

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
                Console.WriteLine($"Error loading accounts: {ex.Message}");
                return new List<PlayerAccount>();
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
            File.WriteAllText(_filePath, jsonData.ToString(Formatting.Indented));
        }

        public JToken FindAccountByUsername(string username)
        {
            var jsonData = ReadJsonFile();
            var accountsArray = jsonData["accounts"] as JArray;
            return accountsArray.FirstOrDefault(acc => acc["Username"].Value<string>() == username);
        }
    }
}

