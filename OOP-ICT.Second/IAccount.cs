namespace lab_2;
public interface IAccount
{
    bool Withdraw(double amount);
    bool Deposit(double amount);
    bool CanAffordBet(double amount);
    double Balance { get; }
}
