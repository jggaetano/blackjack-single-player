using UnityEngine;
using System.Collections;
using System;

public class BustedState : IGameState {

    private readonly DealerController dealer;

    public BustedState(DealerController dealerController)
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
        dealer.DealButton.gameObject.SetActive(false);
        dealer.HitButton.gameObject.SetActive(true);
        dealer.StandButton.gameObject.SetActive(true);
        FirstTime = true;
    }

    public void ToBusted()
    {
        Debug.Log("Can't transition to same state");
    }

    public void ToDealerTurn()
    {
        Debug.Log("Can't transition to Dealer from Busted state");
    }

    public void ToEndGame()
    {
        dealer.currentState = dealer.endGameState;
        Reset();
    }

    public void ToEndHand()
    {
        Debug.Log("Can't transition to EndHand from Busted state");
    }

    public void ToPlayerSplit()
    {
        Debug.Log("Can't transition to PlayerSplit from Busted state");
    }

    public void ToPlayerTurn()
    {
        Debug.Log("Can't transition to Player from Busted state");
    }

    public void ToStartGame()
    {
        Debug.Log("Can't transition to StartGame from Busted state");
    }

    public void ToStartHand()
    {
        dealer.currentState = dealer.startTurnState;
        Reset();
    }

    public void UpdateState(float deltaTime)
    {
        
        if (FirstTime) {
            dealer.DealButton.gameObject.SetActive(true);
            dealer.HitButton.gameObject.SetActive(false);
            dealer.StandButton.gameObject.SetActive(false);

            dealer.GetComponent<DealerController>().RestoreBet();
            
            FirstTime = false;
        }

        // Check if player is out of money
        if (dealer.players[0].GetComponent<PlayerController>().Bank < 2) {
            ToEndGame();
            return;
        }
        
      
    }
}
