using UnityEngine;
using System;

public enum SuitType {
    Spades, Diamonds, Hearts, Clubs
}

public enum ValueType {
    Ace     = 1,
    Two     = 2,
    Three   = 3,
    Four    = 4,
    Five    = 5,
    Six     = 6,
    Seven   = 7,
    Eight   = 8,
    Nine    = 9,
    Ten     = 10,
    Jack    = 11,
    Queen   = 12,
    King    = 13
}

public class Card {

    public Sprite Face { get; private set; }
    public SuitType Suit { get; private set; }
    public ValueType Value { get; private set; }
    public bool FaceUp { get; private set; }
    public string Name {
        get {
            return Value + "_" + Suit;
        }
        private set { }
    }

    Action<Card> cbCardChanged;

    public Card() { }

    public Card(Sprite face, string suit, string value, bool faceUp = true) {
        Face = face;
        Suit = (SuitType)Enum.Parse(typeof(SuitType), suit);
        Value = (ValueType)Enum.Parse(typeof(ValueType), value);
        FaceUp = faceUp;
    }

    public void Flip() {
        FaceUp = !FaceUp;

        if (cbCardChanged != null)
            cbCardChanged(this);
    }

    public void Discard() {
        FaceUp = true;
    }

    public void RegisterOnChangedCallback(Action<Card> cb)
    {
        cbCardChanged += cb;
    }

    public void UnregisterOnChangedCallback(Action<Card> cb)
    {
        cbCardChanged -= cb;
    }

    public override string ToString()
    {
        return "[" + this.Value + " of " + this.Suit + " FaceUp: " + this.FaceUp + "]\n";
    }
}
