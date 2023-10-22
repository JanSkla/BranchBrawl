using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    private InGameUI _inGameUI;

    [SerializeField]
    private GameObject playAgainBtn;

    private NetworkObject _localPlayer;

    private int _alivePlayerCount;
    public int AlivePlayerCount
    {
        get { return _alivePlayerCount; }
        set
        {
            Debug.Log(value + " alive");
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
    void Start()
    {
        _localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
        _localPlayer.transform.Find("Head").GetComponent<PlayerCamera>().OnGameStart();
        _inGameUI = GameObject.Find("InGameUI").GetComponent<InGameUI>();

        if (NetworkManager.IsServer || NetworkManager.IsHost)
        {
            _alivePlayerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            SetAlivePlayerCountClientRpc(AlivePlayerCount);
        }

        State = GameState.Running;
    }
    private void GameSetRunning()
    {
        _inGameUI.UpdateGameScreen(false);
    }
    private void GameOver()
    {
        _inGameUI.UpdateGameScreen(true);
        playAgainBtn.GetComponent<NetworkSuccessBtn>().Fulfilled += StartNewGame;
    }
    private void StartNewGame()
    {
        if (NetworkManager.IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                client.Value.PlayerObject.Despawn();
            }
            //NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
    }
    [ClientRpc]
    private void SetAlivePlayerCountClientRpc(int alivePlayerCount)
    {
        _alivePlayerCount = alivePlayerCount;
        Debug.Log(AlivePlayerCount + " Alive");
    }
}
public enum GameState
{
    Running,
    Over
}
