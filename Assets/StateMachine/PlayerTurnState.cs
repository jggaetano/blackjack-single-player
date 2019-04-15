using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnState : IGameState
{
    private readonly DealerController dealer;
    private Card card;

    public bool FirstTime
    {
        get; set;
    }

    public PlayerTurnState(DealerController dealerController)
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

    public void ToPlayerTurn()
    {
        Debug.Log("Can't transition to same state");
    }

    public void ToStartGame()
    {
        Debug.Log("Can't transition to StartGame from Player state");
    }

    public void ToStartHand()
    {
        dealer.currentState = dealer.startTurnState;
        Reset();
    }

    public void UpdateState(float deltaTime)
    {

        if (FirstTime) {
            dealer.HitButton.gameObject.SetActive(true);
            dealer.StandButton.gameObject.SetActive(true);
            dealer.GetComponent<DealerController>().UpdateBank();
            FirstTime = false;
        }

        if (dealer.players[0].GetComponent<PlayerController>().Hand.cards.Count == 2) {
            // Check if player got a BlackJack
            if (dealer.HasBlackJack(0)) {
                dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text += dealer.players[0].name + " has Blackjack!\n";
                ToDealerTurn();
                return;
            }

            // Check if player can Double Down.
            if (((dealer.Bet * 2) < dealer.players[0].GetComponent<PlayerController>().Bank) && (dealer.players[0].GetComponent<PlayerController>().Hand.Total >= 9 && dealer.players[0].GetComponent<PlayerController>().Hand.Total <= 11))
                dealer.DoubleDownButton.gameObject.SetActive(true);

            // Check if player did Double Down.
            if (dealer.DoublingDown) {
                dealer.GetComponent<DealerController>().SaveBet(); 
                dealer.GetComponent<DealerController>().UpdateBank(dealer.GetComponent<DealerController>().Bet * -1);
                //dealer.GetComponent<DealerController>().Bet *= 2;
                //dealer.BetField.text = dealer.GetComponent<DealerController>().Bet.ToString("0.00");
                card = dealer.DealCard();
                dealer.cardGameObjectMap[card].transform.SetParent(dealer.players[0].GetComponent<PlayerController>().CurrentPlayField.transform);
                dealer.players[0].GetComponent<PlayerController>().Hand.Add(card);
                dealer.players[0].GetComponent<PlayerController>().Hand.Bet *= 2;
                dealer.UpdateTotal(0);
                dealer.DoubleDownButton.gameObject.SetActive(false);
            }

            // Check if player can Split
            if ((PlayerController.Instance.NumberHands < 4) &&
                ((dealer.Bet * 2) < dealer.players[0].GetComponent<PlayerController>().Bank) &&
                (dealer.players[0].GetComponent<PlayerController>().Hand.cards[0].Value == dealer.players[0].GetComponent<PlayerController>().Hand.cards[1].Value))
                dealer.SplitButton.gameObject.SetActive(true);
            else
                dealer.SplitButton.gameObject.SetActive(false);
           
            // Check if player did Split
            if (dealer.Splitting) {
                dealer.GetComponent<DealerController>().UpdateBank();
                PlayerController.Instance.SplitHand();
                dealer.UpdateTotal(0);
                dealer.DoubleDownButton.gameObject.SetActive(false);
                dealer.SplitButton.gameObject.SetActive(false);
                dealer.Splitting = false;
            }
        }
        else {
            dealer.DoubleDownButton.gameObject.SetActive(false);
            dealer.SplitButton.gameObject.SetActive(false);
        }

        // Check if the Player busts, got to end hand.
        if (dealer.players[0].GetComponent<PlayerController>().Hand.Total > 21) {
            dealer.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text += dealer.players[0].name + " Busted!\n";
            if (PlayerController.Instance.NextHand()) {
                ToDealerTurn();
                return;
            }
        }

        if (dealer.DoublingDown && PlayerController.Instance.NextHand()) {
            ToDealerTurn();
            return;
        }

        dealer.DoublingDown = false;

        // Check if the Player stands
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
        Debug.Log("Can't transition to EndGame from Player state");
    }

    public void Reset()
    {
        dealer.HitButton.gameObject.SetActive(false);
        dealer.StandButton.gameObject.SetActive(false);
        dealer.DoubleDownButton.gameObject.SetActive(false);
        dealer.SplitButton.gameObject.SetActive(false);
        FirstTime = true;
    }
}
