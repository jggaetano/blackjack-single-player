using System;
using UnityEngine;
using UnityEngine.UI;


public class EndHandState : IGameState {

    private readonly DealerController dealer;

    public EndHandState(DealerController dealerController)
    {
        dealer = dealerController;
        FirstTime = true;   
    }

    public bool FirstTime
    {
        get; set;
    }

    public void Reset()
    {
        FirstTime = true;
    }

    public void ToDealerTurn()
    {
        Debug.Log("Can't transition to Dealer from EndHand state");
    }

    public void ToEndGame()
    {
        dealer.currentState = dealer.endGameState;
        Reset();
    }

    public void ToEndHand()
    {
        Debug.Log("Can't transition to same state");
    }

    public void ToPlayerTurn()
    {
        Debug.Log("Can't transition to Dealer from EndHand state");
    }

    public void ToStartGame()
    {
        Debug.Log("Can't transition to StartGame from EndHand state");
    }

    public void ToStartHand()
    {
        dealer.currentState = dealer.startTurnState;
        Reset();
    }

    public void UpdateState(float deltaTime)
    {
        // Check to see who won
        dealer.DetermineResults();
        for (int i = 0; i < dealer.DealerIndex; i++) {
            foreach (var hand in dealer.players[i].GetComponent<PlayerController>()._hands) {
                switch (hand.Result) {
                    case Result.BLACKJACK:
                    case Result.WIN:
                        PlayerWins(i, hand.Bet, hand.Result);
                        break;
                    case Result.BUST:
                    case Result.LOSE:
                        DealerWins();
                        break;
                    case Result.PUSH:
                        Push(i, hand.Bet);
                        break;
                }
            }
        }
        /*if (dealer.HasBlackJack(0) && dealer.HasBlackJack(dealer.DealerIndex) == false)
            PlayerWins(0);
        else if (dealer.HasBlackJack(0) == false && dealer.HasBlackJack(dealer.DealerIndex))
            DealerWins();
        else if (dealer.HasBlackJack(0) && dealer.HasBlackJack(dealer.DealerIndex))
            Push();
        else if (dealer.players[0].GetComponent<PlayerController>().Hand.Total > 21)
            DealerWins();
        else if (dealer.players[dealer.DealerIndex].GetComponent<PlayerController>().Hand.Total > 21)
            PlayerWins(0);
        else if (dealer.players[0].GetComponent<PlayerController>().Hand.Total > dealer.players[dealer.DealerIndex].GetComponent<PlayerController>().Hand.Total)
            PlayerWins(0);
        else if (dealer.players[0].GetComponent<PlayerController>().Hand.Total < dealer.players[dealer.DealerIndex].GetComponent<PlayerController>().Hand.Total)
            DealerWins();
        else
            Push(); */


        // Check if player is out of money.
        if (dealer.players[0].GetComponent<PlayerController>().Bank < 2) {
            ToEndGame();
            return;
        }

        ToStartHand();
      
    }

    void PlayerWins(int player, float bet, Result result) {
        float winnings;
        if (result == Result.BLACKJACK)
            winnings = 2.5f * bet;
        else
            winnings = 2 * bet;
        dealer.players[player].GetComponent<PlayerController>().ChangeBank(winnings);
        dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text += dealer.players[player].name + " Wins $" + winnings + "!\n";
    }

    void DealerWins() {
        dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text += dealer.players[dealer.DealerIndex].name + " Wins!\n";
    }

    void Push(int player, float bet) {
        dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text += "Push\n";
        dealer.players[player].GetComponent<PlayerController>().ChangeBank(bet);
    }
}
