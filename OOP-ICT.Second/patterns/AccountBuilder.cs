using System.Threading;

namespace lab_2;

public class AccountBuilder
{
    private static int lastId;
    private string? username;
    private string? email;
    private string? password;
    private double initialBalance;
    private bool isUsernameSet = false;
    private bool isEmailSet = false;
    private bool isPasswordSet = false;

    public AccountBuilder SetUsername(string username)
    {
        this.username = username;
        isUsernameSet = true;
        return this;
    }

    public AccountBuilder SetEmail(string email)
    {
        this.email = email;
        isEmailSet = true;
        return this;
    }

    public AccountBuilder SetPassword(string password)
    {
        this.password = password;
        isPasswordSet = true;
        return this;
    }

    public AccountBuilder SetInitialBalance(double initialBalance)
    {
        this.initialBalance = initialBalance;
        return this;
    }

    public PlayerAccount Build()
    {
        Validate();
        int id = Interlocked.Increment(ref lastId);
        PlayerAccount account = new PlayerAccount(id, username!, email!, password!, initialBalance);
        Reset();
        return account;
    }

    private void Validate()
    {
        if (!isUsernameSet || !isEmailSet || !isPasswordSet)
        {
            throw new InvalidOperationException("Cannot create account, some required fields are not set.");
        }
    }

    public void Reset()
    {
        username = null;
        email = null;
        password = null;
        initialBalance = 0;
        isUsernameSet = false;
        isEmailSet = false;
        isPasswordSet = false;
    }
}
