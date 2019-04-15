using System;
using UnityEngine;
using UnityEngine.UI;

public class StartGameState : IGameState
{
    private readonly DealerController dealer;

    public bool FirstTime
    {
        get; set;
    }

    public StartGameState(DealerController dealerController)
    {
        dealer = dealerController;
        FirstTime = true;
    }

    public void Reset() {
        dealer.StartButton.gameObject.SetActive(false);
        dealer.Starting = false;
        FirstTime = true;
        dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text = "";
    }

    public void ToDealerTurn()
    {
        Debug.Log("Can't transition to Dealer from StartGame state");
    }

    public void ToEndGame()
    {
        Debug.Log("Can't transition to EndGame from StartGame state");
    }

    public void ToEndHand()
    {
        Debug.Log("Can't transition to EndHand from StartGame state");
    }

    public void ToPlayerTurn()
    {
        Debug.Log("Can't transition to Player from StartGame state");
    }

    public void ToStartGame()
    {
        Debug.Log("Can't transition to same state");
    }

    public void ToStartHand()
    {
        dealer.currentState = dealer.startTurnState;
        Reset();
    }

    public void UpdateState(float deltaTime)
    {
        if (FirstTime) {
            dealer.Bet = 0;
            dealer.BetField.text = "";
            dealer.StartButton.gameObject.SetActive(true);
            dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text += "Press Start to begin\n";
            FirstTime = false;
        }

        if (dealer.Starting) {
            dealer.CashIn();
            dealer.Restart();
            ToStartHand();
        }
    }

}
