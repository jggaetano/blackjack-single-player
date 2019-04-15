using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance;

    public GameObject handPlayField;
    public GameObject splitHand2PlayField;
    public GameObject splitHand3PlayField;
    public GameObject splitHand4PlayField;

    //public Hand hand;
    public List<Hand> _hands;

    public float alphaPreset;
    public float alphaHighlight; 

    private float _bank;
    public float Bank
    {
        get { return _bank; }
        set
        {
            _bank = value;
            if (gameObject.tag == "Player")
                gameObject.transform.Find("Table/BankText").gameObject.GetComponent<Text>().text = "Bank: $" + Bank.ToString("0.00");
        }
    }

    public Hand Hand {
        get {
            return _hands[CurrentHand];
        }
        private set {
        }
    }

    public int CurrentHand {

        get; private set;
    }

    public int NumberHands {
        get {
            return _hands.Count;
        }
        set { }
    }

    private GameObject _currentPlayField;
    public GameObject CurrentPlayField {
        get {
            if (_hands.Count == 1)
                return _currentPlayField;
            if (_hands.Count > 1) {
                return _currentPlayField.transform.GetChild(CurrentHand).gameObject;
            }
            return _currentPlayField;
        }
        private set { }
    }
    
    public string Total {
        get {
            string temp = "";
            foreach (var hand in _hands) {
                temp += hand.Total.ToString() + "/"; 
            }
            temp = temp.Substring(0, temp.Length - 1);
            return temp;
        }
        private set { }
    }

    void OnEnable() {

        if (gameObject.tag == "Player") {
            if (Instance != null)
                Debug.LogError("PlayerController instance already created.");

            Instance = this;
        }

    }

    public void Start() {

        //hand = new Hand();
        _hands = new List<Hand>();
        _hands.Add(new Hand());
        CurrentHand = 0;
        Bank = 0;
        _currentPlayField = handPlayField;

        /*if (gameObject.tag == "Player") { 
            Bank = 100;
            gameObject.transform.Find("Table/BankText").gameObject.GetComponent<Text>().text = "Bank: $" + Bank;
        }*/

    }

    public void SplitHand() {

        // Update Data
        // Add another hand
        Hand newHand = new Hand();
        newHand.Bet = DealerController.Instance.Bet;
        _hands.Add(newHand);
    
        // Move card to last hand.
        _hands[_hands.Count-1].cards.Add(_hands[CurrentHand].cards[1]);

        // Remove card from current hand.
        _hands[CurrentHand].cards.Remove(_hands[CurrentHand].cards[1]);

        // Deal Cards
        _hands[CurrentHand].Add(DealerController.Instance.DealCard());
        _hands[_hands.Count - 1].Add(DealerController.Instance.DealCard());

        // Active Split Playing Fields
        GameObject oldPlayField = null;
        switch (_hands.Count) {
            case 2:
                MakeBatman(_currentPlayField);
                oldPlayField = handPlayField; 
                _currentPlayField = splitHand2PlayField;
                break;
            case 3:
                MakeBatman(_currentPlayField.transform.GetChild(0).gameObject);
                MakeBatman(_currentPlayField.transform.GetChild(1).gameObject);
                oldPlayField = splitHand2PlayField;
                _currentPlayField = splitHand3PlayField;
                break;
            case 4:
                MakeBatman(_currentPlayField.transform.GetChild(0).gameObject);
                MakeBatman(_currentPlayField.transform.GetChild(1).gameObject);
                MakeBatman(_currentPlayField.transform.GetChild(2).gameObject);
                oldPlayField = splitHand3PlayField;
                _currentPlayField = splitHand4PlayField;
                break;
            default:
                break;
        }
        _currentPlayField.SetActive(true);

        // Hide Old Play Fields
        oldPlayField.SetActive(false);

        // Add Cards to PlayFields
        int saveCurrent = CurrentHand;
        CurrentHand = 0;
        for (int i = 0; i < _hands.Count; i++) {
            for (int j = 0; j < _hands[i].cards.Count; j++) {
                DealerController.Instance.cardGameObjectMap[_hands[i].cards[j]].transform.SetParent(CurrentPlayField.transform);
            }
            NextHand(false);
        }
        CurrentHand = saveCurrent;

        HighlightHand();
    }

    public void UnSplitHand() {
        _hands.Clear();
        _hands.Add(new Hand());
        CurrentHand = 0;
        handPlayField.SetActive(true);
        splitHand2PlayField.SetActive(false);
        splitHand3PlayField.SetActive(false);
    }

    public bool NextHand(bool highlight = true) {
        if (_hands.Count > 1 && highlight)
            UnHighlightHand();

        CurrentHand = (CurrentHand + 1) % _hands.Count;
        if (CurrentHand == 0)
            return true;

        if (_hands.Count > 1 && highlight)
            HighlightHand();

        return false;
    }

    void UnHighlightHand() {
        Color c = CurrentPlayField.GetComponent<Image>().color;
        c.a = alphaPreset;
        CurrentPlayField.GetComponent<Image>().color = c;
    }

    void HighlightHand() {
        Color c = CurrentPlayField.GetComponent<Image>().color;
        c.a = alphaHighlight;
        CurrentPlayField.GetComponent<Image>().color = c;
    }

    void MakeBatman(GameObject bruceWayne) {
        for (int i = bruceWayne.transform.childCount - 1; i >= 0; i--) {
            bruceWayne.transform.GetChild(i).SetParent(null);
        }
    }

    public void ChangeBank(float bank) {
        _bank += bank;
        gameObject.transform.Find("Table/BankText").gameObject.GetComponent<Text>().text = "Bank: $" + Bank.ToString("0.00");
    }

    

    /*public override string ToString()
    {
        string handString = "Hand: ";
        foreach (var card in cards)
        {
            handString += card.ToString();
        }
        return handString;
    }

    /*public GameObject tablePrefab; 
    public GameObject handPrefab;
    GameObject handGameObject;*/

    /* gameObject.name = "Player " + NetworkServer.connections.Count.ToString();

     GameObject tableGameObject = (GameObject)Instantiate(tablePrefab);
     handGameObject = (GameObject)Instantiate(handPrefab, handPrefab.transform.position, handPrefab.transform.rotation);

     Debug.Log(gameObject.GetComponentInChildren<Camera>().gameObject.name);
     tableGameObject.GetComponent<Canvas>().worldCamera = gameObject.GetComponentInChildren<Camera>();
     tableGameObject.transform.SetParent(this.transform);
     tableGameObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
     {
         DealerController.instance.CmdDealCard();
     });

     tableGameObject.name = "Table";

     handGameObject.name = "Hand";
     handGameObject.transform.SetParent(tableGameObject.transform, false);

     RectTransform rt = handGameObject.GetComponent<RectTransform>();
     rt.anchorMin = new Vector2(0, 0);
     rt.anchorMax = new Vector2(1, 0);
     rt.pivot = new Vector2(0.5f, 0);

     rt.localScale = new Vector3(1, 1, 1);
     rt.localPosition = new Vector3(0, 0, 0);
     rt.anchoredPosition = new Vector2(0, 0);
     rt.sizeDelta = new Vector2(0, 120);*/



}
