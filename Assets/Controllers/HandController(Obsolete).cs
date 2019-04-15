using UnityEngine;
using System.Collections.Generic;

public class HandController : MonoBehaviour {

    public static HandController Instance { get; protected set; }
    Dictionary<Hand, GameObject> handGameObjectMap;

    void OnEnable() {

        if (Instance != null)
            Debug.LogError("There should never be two HandControllers.");

        Instance = this;

    }

    void Start() {

        handGameObjectMap = new Dictionary<Hand, GameObject>();
 
    }

    public void NewHand(Hand hand, GameObject handGameObject) {

        handGameObjectMap.Add(hand, handGameObject);

    }

    public void AddCard() {



    }

    void OnHandCreated(Hand hand) { 

    }

    void OnHandChanged(Hand hand) { 
        

    }

}
