namespace lab_2;
public interface IAccount
{
    bool Withdraw(double amount);
    void Deposit(double amount);
    bool CanAffordBet(double amount);
}

// Реализация интерфейса банковского счета
public class PlayerAccount : IAccount
{
    private double balance;

    public PlayerAccount(double initialBalance)
    {
        balance = initialBalance;
    }

    public bool Withdraw(double amount)
    {
        if (amount <= balance)
        {
            balance -= amount;
            return true;
        }
        return false;
    }

    public void Deposit(double amount)
    {
        balance += amount;
    }

    public bool CanAffordBet(double amount)
    {
        return amount <= balance;
    }
    public double Balance
    {
        get { return balance; }
    }

}
