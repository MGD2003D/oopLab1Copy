
namespace lab_2;


// Обновленная реализация интерфейса банковского счета с дополнительными полями
public class PlayerAccount : IAccount
{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public double Balance { get; private set; }

    public PlayerAccount(int id, string username, string email, string password, double initialBalance)
    {
        Id = id;
        Username = username;
        Email = email;
        Password = password;
        Balance = initialBalance;
    }

    public bool Withdraw(double amount)
    {
        if (amount <= Balance)
        {
            Balance -= amount;
            return true;
        }
        return false;
    }

    public bool Deposit(double amount)
    {
        if (amount > 0)
        {
            Balance += amount;
            return true;
        }
        return false;
    }

    public bool CanAffordBet(double amount)
    {
        return amount <= Balance;
    }
}
