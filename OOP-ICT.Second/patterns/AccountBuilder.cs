
using System.Threading;

namespace lab_2;

public class AccountBuilder
{
    private static int lastId;
    private string username;
    private string email;
    private string password;
    private double initialBalance;

    public AccountBuilder SetUsername(string username)
    {
        this.username = username;
        return this;
    }

    public AccountBuilder SetEmail(string email)
    {
        this.email = email;
        return this;
    }

    public AccountBuilder SetPassword(string password)
    {
        this.password = password;
        return this;
    }

    public AccountBuilder SetInitialBalance(double initialBalance)
    {
        this.initialBalance = initialBalance;
        return this;
    }

    public PlayerAccount Build()
    {
        int id = Interlocked.Increment(ref lastId);
        return new PlayerAccount(id, username, email, password, initialBalance);
    }
}
