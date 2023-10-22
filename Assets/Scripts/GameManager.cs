using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private InGameUI _inGameUI;

    private NetworkObject _localPlayer;

    private int _playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
    private int _alivePlayerCount;
    public int AlivePlayerCount
    {
        get { return _alivePlayerCount; }
        set
        {
            _alivePlayerCount = value;
            if (_alivePlayerCount <= 1)
            {
                State = GameState.Over;
            }
        }
    }

    private GameState _state;
    public GameState State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (value)
            {
                case GameState.Running:
                    GameSetRunning();
                    return;
                case GameState.Over:
                    GameOver();
                    return;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _alivePlayerCount = _playerCount;
        _localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
        _localPlayer.transform.Find("Head").GetComponent<PlayerCamera>().OnGameStart();
        _inGameUI = GameObject.Find("InGameUI").GetComponent<InGameUI>();
        State = GameState.Running;
    }
    private void GameSetRunning()
    {
        _inGameUI.UpdateGameScreen(false);
    }
    private void GameOver()
    {
        _inGameUI.UpdateGameScreen(true);
    }
}
public enum GameState
{
    Running,
    Over
}
