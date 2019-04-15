using System;
using UnityEngine;
using UnityEngine.UI;

public class StartTurnState : IGameState
{

    private readonly DealerController dealer;
    private Card card;

    public bool FirstTime
    {
        get; set;
    }

    public StartTurnState(DealerController dealerController) {
        dealer = dealerController;
        FirstTime = true;
    }

    public void Reset() {
        dealer.DealButton.gameObject.SetActive(false);
        dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text = "";
        FirstTime = true;
    }

    public void ToDealerTurn()
    {
        Debug.Log("Can't transition to Dealer from StartHand state");
    }

    public void ToEndHand()
    {
        dealer.currentState = dealer.endHandState;
        Reset();
    }

    public void ToPlayerTurn()
    {
        dealer.currentState = dealer.playerTurnState;
        Reset();
    }

    public void ToStartGame()
    {
        Debug.Log("Can't transition to StartGame state from StartHand state.");
    }

    public void ToStartHand()
    {
        Debug.Log("Can't transition to same state");
    }

    public void UpdateState(float deltaTime)
    {
        if (FirstTime) { 
            dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text += "Press deal to begin\n";
            dealer.DealButton.gameObject.SetActive(true);
            dealer.PlayerStands = false;
            dealer.Dealing = false;
            dealer.DoublingDown = false;
            dealer.Splitting = false;
            dealer.BetField.enabled = true;
            dealer.RestoreBet();
            dealer.players[0].GetComponent<PlayerController>().UnSplitHand();
            FirstTime = false;
        }

        if (dealer.Dealing) {
            dealer.Restart();

            // Deal two cards to everyone
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < dealer.players.Count; j++)
                {
                    if (j == (dealer.players.Count - 1)) {
                        if (i == 1)  
                            card = dealer.DealCard();
                        else  
                            card = dealer.DealCard(false);
                    }
                    else 
                        card = dealer.DealCard();
                
                    dealer.cardGameObjectMap[card].transform.SetParent(dealer.players[j].transform.FindChild("Table/Hand").transform);
                    dealer.players[j].GetComponent<PlayerController>().Hand.Add(card);
                    dealer.UpdateTotal(j);
                }
            }

            // Check if Dealer has a blackjack.
            if (dealer.HasBlackJack(dealer.DealerIndex))
            {
                dealer.UpdateBank();
                dealer.players[dealer.DealerIndex].GetComponent<PlayerController>().Hand.FlipUp();
                dealer.UpdateTotal(dealer.DealerIndex);
                dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text = dealer.players[dealer.DealerIndex].name + " has Blackjack!\n";
                ToEndHand();
                return;
            }

            ToPlayerTurn();
        }
    }

    public void ToEndGame()
    {
        Debug.Log("Can't transition to EndGame state from StartHand state.");
    }

}
