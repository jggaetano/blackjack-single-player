using UnityEngine;
using System.Collections.Generic;

public class Deck {

    public int Size {
        get {
            return cards.Count;
        }
        private set { }
    }

    public int Remaining {
        get {
            return shuffledCards.Count;
        }
        private set { }
    }

    private Queue<Card> cards;
    private Queue<Card> shuffledCards;

    public Deck() { }

    public Deck(Sprite[] cardFaces, int numberOfDecks = 1) {

        cards = new Queue<Card>();
        shuffledCards = new Queue<Card>();
        string[] cardData;

        for (int i = 0; i < numberOfDecks; i++) {

            foreach (Sprite face in cardFaces) {
                cardData = face.name.Split('_');
                if (cardData.Length == 2)
                    cards.Enqueue(new Card(face, cardData[1], cardData[0]));
            }
        }
        Shuffle();
    }

    public void Shuffle() {

        List<Card> temp = new List<Card>();
        Card swap = new Card();
        int pos1, pos2;

        while (cards.Count > 0) {
            temp.Add(cards.Dequeue());
        }

        for (int i = 0; i < 750; i++) {
            pos1 = Random.Range(0, temp.Count);
            pos2 = Random.Range(0, temp.Count);

            swap = temp[pos1];
            temp[pos1] = temp[pos2];
            temp[pos2] = swap;
        }

        //shuffledCards.Enqueue(new Card(null, "Hearts", "Three"));
        //shuffledCards.Enqueue(new Card(null, "Hearts", "Three"));
        //shuffledCards.Enqueue(new Card(null, "Diamonds", "Three"));
        //shuffledCards.Enqueue(new Card(null, "Diamonds", "Three"));
        //shuffledCards.Enqueue(new Card(null, "Clubs", "Three"));
        //shuffledCards.Enqueue(new Card(null, "Clubs", "Three"));
        

        foreach (Card card in temp) {
            shuffledCards.Enqueue(card);
        }

    }

    public Card Deal() {

        if (shuffledCards == null || shuffledCards.Count == 0) {
            Debug.LogError("Deck not shuffled, returning null Card");
            return null;
        }

        return shuffledCards.Dequeue();
    }

    public void Discard(Card card) { 
        card.Discard();
        cards.Enqueue(card);
    }

    public override string ToString() {
        string deckString = "Deck: ";
        foreach (var card in cards)
        {
            deckString += card.ToString();
        }
        return deckString;
    }
}
