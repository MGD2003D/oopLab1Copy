namespace lab_2;

public class BlackjackCasino
{
    private const double BlackjackPayoutRatio = 1.5; // Соотношение выплаты при блэкджеке

    public void AwardWin(IAccount playerAccount, double betAmount)
    {
        // Начисление обычного выигрыша 1:1
        playerAccount.Deposit(betAmount * 2);
    }

    public void HandleBlackjack(IAccount playerAccount, double betAmount)
    {
        // Начисление выигрыша при блэкджеке
        playerAccount.Deposit(betAmount * BlackjackPayoutRatio + betAmount);
    }
}
