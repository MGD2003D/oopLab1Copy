using lab_2;
using Newtonsoft.Json.Linq;
using OOP_ICT.Models;

namespace lab_3;

public class BlackjackGame
{
    private readonly AccountService _accountService;

    public BlackjackGame(AccountService accountService)
    {
        _accountService = accountService;
    }

    public bool PlayerConnect(string username, string password)
    {
        var account = _accountService.GetAccountByCredentials(username, password);
        if (account != null)
        {
            Console.WriteLine($"{username} connected to the game.");
            return true;
        }
        else
        {
            Console.WriteLine("Invalid credentials.");
            return false;
        }
    }

    public void StartGame(string username, int betAmount)
    {
        var dealer = new Dealer();
        dealer.PerfectShuffle();

        var playerHand = new List<Card> { dealer.DealCard(), dealer.DealCard() };
        var dealerHand = new List<Card> { dealer.DealCard() };

        int playerScore = playerHand.Sum(card => BlackjackCardValue.CalculateCardValue(card));
        int dealerScore = dealerHand.Sum(card => BlackjackCardValue.CalculateCardValue(card));
        playerScore = BlackjackCardValue.AdjustForAces(playerHand, playerScore);
        dealerScore = BlackjackCardValue.AdjustForAces(dealerHand, dealerScore);

        var accountData = _accountService.ReadJsonFile();
        var accountsArray = (JArray)accountData["accounts"];
        var account = accountsArray.FirstOrDefault(acc => acc["Username"].Value<string>() == username);

        if (account != null)
        {
            account["nowplaying"] = "blackjack";
            account["bet"] = betAmount;
            account["PlayerScore"] = playerScore;
            account["DealerScore"] = dealerScore;
            account["isPlayerStopped"] = false;
            _accountService.WriteJsonFile(accountData);
        }
    }

    public void EndGame(string username)
    {
        var accountData = _accountService.ReadJsonFile();
        var accountsArray = (JArray)accountData["accounts"];
        var account = accountsArray.FirstOrDefault(acc => acc["Username"].Value<string>() == username);

        if (account != null)
        {
            var propertiesToRemove = new List<string> { "nowplaying", "bet", "PlayerScore", "DealerScore", "isPlayerStopped" };
            foreach (var prop in propertiesToRemove)
            {
                if (account[prop] != null)
                {
                    account[prop].Parent.Remove();
                }
            }

            _accountService.WriteJsonFile(accountData);
            Console.WriteLine($"Game ended for {username}.");
        }
    }

    public void PlayerTurn(string username)
    {
        var accountData = _accountService.ReadJsonFile();
        var accountsArray = (JArray)accountData["accounts"];
        var account = accountsArray.FirstOrDefault(acc => acc["Username"].Value<string>() == username);

        if (account != null)
        {
            var dealer = new Dealer();
            dealer.PerfectShuffle();
            var card = dealer.DealCard();
            var playerScore = account["PlayerScore"].Value<int>() + BlackjackCardValue.CalculateCardValue(card);
            playerScore = BlackjackCardValue.AdjustForAces(new List<Card> { card }, playerScore);

            account["PlayerScore"] = playerScore;
            _accountService.WriteJsonFile(accountData);
        }
    }
    public void DealerTurn(string username)
    {
        var accountData = _accountService.ReadJsonFile();
        var accountsArray = (JArray)accountData["accounts"];
        var account = accountsArray.FirstOrDefault(acc => acc["Username"].Value<string>() == username);

        if (account != null)
        {
            var dealer = new Dealer();
            dealer.PerfectShuffle();
            var card = dealer.DealCard();
            var dealerScore = account["DealerScore"].Value<int>() + BlackjackCardValue.CalculateCardValue(card);
            dealerScore = BlackjackCardValue.AdjustForAces(new List<Card> { card }, dealerScore);

            account["DealerScore"] = dealerScore;
            _accountService.WriteJsonFile(accountData);
        }
    }
    public string CheckWin(string username)
    {
        var accountData = _accountService.ReadJsonFile();
        var accountsArray = (JArray)accountData["accounts"];
        var account = accountsArray.FirstOrDefault(acc => acc["Username"].Value<string>() == username);

        if (account != null)
        {
            int playerScore = account["PlayerScore"].Value<int>();
            int dealerScore = account["DealerScore"].Value<int>();
            bool isPlayerStopped = account["isPlayerStopped"].Value<bool>();

            if (playerScore < 21 && dealerScore < 21 && !isPlayerStopped)
            {
                return "in progress";
            }

            if (playerScore > 21) return "lose";
            if (dealerScore > 21) return "win";
            if (playerScore == dealerScore) return "draw";
            if (playerScore == 21) return "win";
            if (dealerScore == 21 || isPlayerStopped) return "lose";
            return playerScore > dealerScore ? "win" : "lose";
        }

        return "error";
    }



    public void Reward(string username)
    {
        var accountData = _accountService.ReadJsonFile();
        var accountsArray = (JArray)accountData["accounts"];
        var account = accountsArray.FirstOrDefault(acc => acc["Username"].Value<string>() == username);

        if (account != null)
        {
            string result = CheckWin(username);
            double betAmount = account["bet"].Value<double>();
            double balance = account["Balance"].Value<double>();

            if (result == "win")
            {
                account["Balance"] = balance + betAmount;
            }
            else if (result == "lose")
            {
                account["Balance"] = balance - betAmount;
            }

            _accountService.WriteJsonFile(accountData);
        }
    }
    public void PlayerStopped(string username)
    {
        var accountData = _accountService.ReadJsonFile();
        var accountsArray = (JArray)accountData["accounts"];
        var account = accountsArray.FirstOrDefault(acc => acc["Username"].Value<string>() == username);

        if (account != null)
        {
            account["isPlayerStopped"] = true;
            _accountService.WriteJsonFile(accountData);
        }
    }

}