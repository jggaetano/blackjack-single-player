public interface IGameState {

    bool FirstTime { get; set; }
    void UpdateState(float deltaTime);
    void Reset();
    void ToStartGame();
    void ToStartHand();
    void ToPlayerTurn();
    void ToDealerTurn();
    void ToEndHand();
    void ToEndGame();

}
