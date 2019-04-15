using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSplitState : IGameState
{

    private readonly DealerController dealer;
    private List<List<Card>> hands;

    public bool FirstTime
    {
        get; set;
    }

    //private int currentHand = 0;

    public PlayerSplitState(DealerController dealerController)
    {
        dealer = dealerController;
        FirstTime = true;
    }

    public void ToDealerTurn()
    {
        dealer.currentState = dealer.dealerTurnState;
        Reset();
    }

    public void ToEndHand()
    {
        dealer.currentState = dealer.endHandState;
        Reset();
    }

    public void ToPlayerSplit()
    {
        Debug.Log("Can't transition to same state");
    }

    public void ToPlayerTurn()
    {
        dealer.currentState = dealer.playerTurnState;
        Reset();
    }

    public void ToStartGame()
    {
        Debug.Log("Can't transition to Start from PlayerSplit state");
    }

    public void ToStartHand()
    {
        dealer.currentState = dealer.startTurnState;
        Reset();
    }

    public void UpdateState(float deltaTime)
    {

        if (FirstTime)
        {
            dealer.HitButton.gameObject.SetActive(true);
            dealer.StandButton.gameObject.SetActive(true);
            FirstTime = false;
        }

        // Check if the Player busts, got to end hand.
        if (dealer.players[0].GetComponent<PlayerController>().Hand.Total > 21)
        {
            dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text += dealer.players[0].name + " Busted!\n";
            if (PlayerController.Instance.NextHand()) {
                ToEndHand();
                return;
            }
        }

        if (dealer.PlayerStands) {
            dealer.PlayerStands = false;
            // If Next Hand is the first hand, move on.
            if (PlayerController.Instance.NextHand()) {
                ToDealerTurn();
                return;
            }
        }   

    }

    public void ToEndGame()
    {
        Debug.Log("Can't transition to EndGame from PlayerSplit state");
    }

    public void Reset()
    {
        dealer.HitButton.gameObject.SetActive(false);
        dealer.StandButton.gameObject.SetActive(false);
        FirstTime = true;
    }
}
