using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public enum Result {
    BLACKJACK, BUST, WIN, LOSE, PUSH
}

public class DealerController : MonoBehaviour
{
    public static DealerController Instance;

    public GameObject cardPrefab;
    public Sprite[] cardFaces;
    public bool PlayerStands { get; set; }
    public bool Dealing { get; set; }
    public bool DoublingDown { get; set; }
    public bool Splitting { get; set; }
    public bool Starting { get; set; }
    public float Bet { get; set; }
    public int DealerIndex {
        get {
            return players.Count - 1;
        }
        private set { }
    }
    public Button StartButton;
    public Button DealButton;
    public Button HitButton;
    public Button StandButton;
    public Button DoubleDownButton;
    public Button SplitButton;
    public InputField BetField;
    public Dictionary<Card, GameObject> cardGameObjectMap;

    private Deck shoe;
    private float _saveBet;


    [HideInInspector]
    public List<GameObject> players;
    [HideInInspector]
    public IGameState currentState;
    [HideInInspector]
    public StartGameState startGameState;
    [HideInInspector]
    public StartTurnState startTurnState;
    [HideInInspector]
    public DealerTurnState dealerTurnState;
    [HideInInspector]
    public PlayerTurnState playerTurnState;
    [HideInInspector]
    public EndHandState endHandState;
    [HideInInspector]
    public EndGameState endGameState;

    void OnEnable() {

        if (Instance != null)
            Debug.LogError("DealController instance already created.");

        Instance = this;

    }

    void Awake() {

        startGameState = new StartGameState(this);
        startTurnState = new StartTurnState(this);
        playerTurnState = new PlayerTurnState(this);
        dealerTurnState = new DealerTurnState(this);
        endHandState = new EndHandState(this);
        endGameState = new EndGameState(this);

    }

    void Start() {

        cardGameObjectMap = new Dictionary<Card, GameObject>();
        shoe = new Deck(cardFaces, 2);
        currentState = startGameState;

        players = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();
        players.Add( GameObject.FindGameObjectWithTag("Dealer") );
        gameObject.transform.Find("Table/GameStatusText").gameObject.GetComponent<Text>().text = "";
        GameObject.Find("RemainingText").GetComponent<Text>().text = "Remaining: " + shoe.Remaining;
        StartButton.gameObject.SetActive(true);
        DealButton.gameObject.SetActive(false);
        HitButton.gameObject.SetActive(false);
        StandButton.gameObject.SetActive(false);
        DoubleDownButton.gameObject.SetActive(false);
        SplitButton.gameObject.SetActive(false);
        BetField.enabled = false;

    }

    void Update() {
        currentState.UpdateState(Time.deltaTime);
    }

    public void Deal() {
        if (BetField.text == "")
            BetField.text = "0";

        float newBet;
        if (float.TryParse(BetField.text, out newBet)) {
            Bet = newBet;
        }
        else {
            return;
        }

        if (Bet >= 2 && Bet <= players[0].GetComponent<PlayerController>().Bank && Bet <= 100) {
            players[0].GetComponent<PlayerController>().Hand.Bet = Bet;
            BetField.enabled = false;
            Dealing = true;
        }
    }

    public void UpdateBank() {
        players[0].GetComponent<PlayerController>().ChangeBank(-1 * Bet);
    }

    public void UpdateBank(float value) {
        players[0].GetComponent<PlayerController>().ChangeBank(value);
    }

    public void SaveBet() {
        _saveBet = Bet;
    }

    public void RestoreBet() {
        if (_saveBet != 0) {
            Bet = _saveBet;
            _saveBet = 0;
        }
        BetField.text = Bet.ToString("0.00");
    }

    public void Hit(int player) {

        if (players.Count == 0)
            Debug.LogError("No Players - Huge Problem");

        Card card = DealCard();
        cardGameObjectMap[card].transform.SetParent(players[player].GetComponent<PlayerController>().CurrentPlayField.transform);
        players[player].GetComponent<PlayerController>().Hand.Add(card);
        UpdateTotal(player);
    }

    public void Stand() {
        PlayerStands = true;
    }

    public void DoubleDown() {
        DoublingDown = true;
    }

    public void Split() {
        Splitting = true;
    }

    public void StartGame() {
        Starting = true;
    }
    
    public Card DealCard(bool faceUp = true) {
   
        GameObject cardGameObject = (GameObject)Instantiate(cardPrefab);
        Card card = shoe.Deal();
        card.RegisterOnChangedCallback(OnCardChanged);

        if (cardGameObjectMap.ContainsKey(card))
            cardGameObjectMap[card] = cardGameObject;
        else
            cardGameObjectMap.Add(card, cardGameObject);

        if (shoe.Remaining == 0) {
            Debug.Log("Shuffling");
            shoe.Shuffle();
        }

        cardGameObject.name = card.Value.ToString() + "_" + card.Suit.ToString();
        cardGameObject.GetComponent<Image>().sprite = card.Face;
        if (faceUp == false)
            card.Flip();

        return card;

        //cardGameObject.transform.SetParent(players[player].transform.FindChild("Table/Hand").transform);
        //players[player].GetComponent<PlayerController>().TakeCard(card);
        //UpdateTotal(player);

        /*cardGameObject.transform.localScale = Vector3.one;
        RectTransform rt = cardGameObject.GetComponent<RectTransform>();
        rt.localScale = new Vector3(1, 1, 1);
        rt.localPosition = new Vector3(0, 0, 0);
        rt.anchoredPosition = new Vector2(0, 0);*/

    }

    public void UpdateTotal(int player) {

        if (players == null) {
            Debug.LogError("players are null!");
            return;
        }
        if (players.Count == 0) {
            Debug.LogError("No players?");
            return;
        }

        players[player].transform.FindChild("Table/TotalText").gameObject.GetComponent<Text>().text = "Total: " + players[player].GetComponent<PlayerController>().Total;
        GameObject.Find("RemainingText").GetComponent<Text>().text = "Remaining: " + shoe.Remaining;
    }

    void Discard(GameObject player) {

        foreach (var card in player.GetComponent<PlayerController>().Hand.cards) {
            shoe.Discard(card);
        }
        player.GetComponent<PlayerController>().Hand.Discard();
    }

    public void Restart() {

        GameObject hand;

        foreach (var player in players) {
            Discard(player);
            hand = player.transform.FindChild("Table/Hand").gameObject;
            for (int i = hand.transform.childCount-1; i >= 0; i--) {
                Destroy(hand.transform.GetChild(i).gameObject);
            }
        }

    }

    public bool HasBlackJack(int player) {
        return HasBlackJack(players[player].GetComponent<PlayerController>().Hand);
    }

    public bool HasBlackJack(Hand hand) {

        bool ace = false, ten = false;

        foreach (var card in hand.cards) {
            if (card.Value == ValueType.Ace)
                ace = true;
            if (card.Value == ValueType.Ten ||
                card.Value == ValueType.Jack ||
                card.Value == ValueType.Queen ||
                card.Value == ValueType.King)
                ten = true;
        }

        return (ace && ten && (hand.NumberOfCards == 2));
    }

    public void DetermineResults() {

        Result dealerResult = Result.PUSH;

        int dealerTotal = players[DealerIndex].GetComponent<PlayerController>().Hand.Total;
        if (dealerTotal > 21)
            dealerResult = Result.BUST;
        if (HasBlackJack(DealerIndex))
            dealerResult = Result.BLACKJACK;

        for (int i = 0; i < DealerIndex; i++) {
            foreach (var hand in players[i].GetComponent<PlayerController>()._hands) {
                if (dealerResult == Result.BLACKJACK) {
                    if (HasBlackJack(hand))
                        hand.Result = Result.PUSH;
                    else
                        hand.Result = Result.LOSE;
                }
                else if (dealerResult != Result.BUST) {
                    if (HasBlackJack(hand))
                        hand.Result = Result.BLACKJACK;
                    else if (hand.Total > 21)
                        hand.Result = Result.BUST;
                    else if (hand.Total < dealerTotal)
                        hand.Result = Result.LOSE;
                    else if (hand.Total > dealerTotal)
                        hand.Result = Result.WIN;
                    else
                        hand.Result = Result.PUSH;
                }
                else
                    hand.Result = Result.WIN;
           }
        }

    }

    public void CashIn() {

        foreach (var player in players) {
            if (player.tag == "Player") 
                player.GetComponent<PlayerController>().Bank = 100;
        }

    }

    void OnCardChanged(Card card) {

        if (cardGameObjectMap.ContainsKey(card) == false)
        {
            Debug.LogError("OnCardChanged -- trying to change visuals for character not in our map.");
            return;
        }

        GameObject cardGameObject = cardGameObjectMap[card];
        if (card.FaceUp)
            cardGameObject.GetComponent<Image>().sprite = card.Face;
        else
            cardGameObject.GetComponent<Image>().sprite = null;
    }

}
