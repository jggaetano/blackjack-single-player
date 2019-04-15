using UnityEngine;
using System;
using System.Collections.Generic;

public class Hand {

    public List<Card> cards;

    public float Bet { get; set; }
    public Result Result { get; set; }

    public int Total {
        get {
            int _total = 0;
            int _aces = 0;
            foreach (Card card in cards) {
                if (card.FaceUp) {
                    if (card.Value == ValueType.Ace) {
                        _aces++;
                        _total += 11;
                    }
                    else if (card.Value == ValueType.Jack ||
                             card.Value == ValueType.Queen ||
                             card.Value == ValueType.King)
                        _total += 10;
                    else
                        _total += card.Value.GetHashCode();
                }
            }
            while (_total > 21) {
                if (_aces > 0) {
                    _aces--;
                    _total -= 10;
                }
                else
                    break;
            }
            return _total;
        }
        private set { }
    }

    public int NumberOfCards {
        get {
            return cards.Count;
        }
        protected set { }
    }

    public Hand() {
        cards = new List<Card>();
        Bet = 0;
        Result = Result.PUSH;
    }

    public void Add(Card card) {
        cards.Add(card);
    }

    public void Discard() {
        cards.Clear();
    }

    public void FlipUp() {

        foreach (var card in cards) {
            if (card.FaceUp == false) {
                card.Flip();
            }
        }

    }

}
