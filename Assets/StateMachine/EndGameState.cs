using System;
using UnityEngine;
using UnityEngine.UI;

public class EndGameState : IGameState {

    private readonly DealerController dealer;

    public EndGameState(DealerController dealerController) {
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
        Debug.Log("Can't transition to Dealer from EndGame state");
    }

    public void ToEndGame()
    {
        Debug.Log("Can't transition to same state");
    }

    public void ToEndHand()
    {
        Debug.Log("Can't transition to EndHand from EndGame state");
    }

    public void ToPlayerTurn()
    {
        Debug.Log("Can't transition to Player from EndGame state");
    }

    public void ToStartGame()
    {
        dealer.currentState = dealer.startGameState;
        Reset();
    }

    public void ToStartHand()
    {
        Debug.Log("Can't transition to StartHand from EndGame state");
    }

    public void UpdateState(float deltaTime)
    {
        dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text += "You are broke!\nGame Over!\n";
        ToStartGame();

    }

}
