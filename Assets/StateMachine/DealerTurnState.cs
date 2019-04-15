using System;
using UnityEngine;
using UnityEngine.UI;

public class DealerTurnState : IGameState
{
    private readonly DealerController dealer;
    private float coolDown = 1.0f;
    private float timer = 1.0f;
    private Card card;

    public bool FirstTime
    {
        get; set;
    }

    public DealerTurnState(DealerController dealerController)
    {
        dealer = dealerController;
        FirstTime = true;
    }

    public void ToDealerTurn()
    {
        Debug.Log("Can't transition to same state");
    }

    public void ToEndHand()
    {
        dealer.currentState = dealer.endHandState;
        Reset();
    }

    public void ToPlayerTurn()
    {
        Debug.Log("Can't transition to Player turn from Dealer Turn");
    }

    public void ToStartGame()
    {
        Debug.Log("Can't transition to StartGame turn from Dealer Turn");
    }

    public void ToStartHand()
    {
        dealer.currentState = dealer.startTurnState;
        Reset();
    }

    public void UpdateState(float deltaTime)
    {

        if (FirstTime) { 
            dealer.players[dealer.DealerIndex].GetComponent<PlayerController>().Hand.FlipUp();
            dealer.UpdateTotal(dealer.DealerIndex);
            FirstTime = false;
        }

        if (dealer.players[dealer.DealerIndex].GetComponent<PlayerController>().Hand.Total >= 17) {
            ToEndHand();
            return;
        }

        timer -= deltaTime;
        if (timer <= 0) { 
            card = dealer.DealCard();
            dealer.cardGameObjectMap[card].transform.SetParent(dealer.players[dealer.DealerIndex].transform.FindChild("Table/Hand").transform);
            dealer.players[dealer.DealerIndex].GetComponent<PlayerController>().Hand.Add(card);
            dealer.UpdateTotal(dealer.DealerIndex);
            timer = coolDown;
        }
        
        // Check if the Player busts, got to end hand.
        if (dealer.players[dealer.DealerIndex].GetComponent<PlayerController>().Hand.Total > 21) {
            dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text += dealer.players[dealer.DealerIndex].name + " Busted!\n";
            ToEndHand();
            return;
        }
    }

    public void ToEndGame()
    {
        Debug.Log("Can't transition to EndGame turn from Dealer Turn");
    }

    public void Reset()
    {
        FirstTime = true;
    }
}
