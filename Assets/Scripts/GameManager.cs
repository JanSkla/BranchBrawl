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
        StartGame();
    }
    private void GameSetRunning()
    {
        _inGameUI.UpdateGameScreen(false);
    }
    private void GameOver()
    {
        _inGameUI.UpdateGameScreen(true);
        playAgainBtn.GetComponent<NetworkSuccessBtn>().Fulfilled += PlayAgain;
    }
    private void StartGame()
    {
        if (NetworkManager.IsServer || NetworkManager.IsHost)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                client.Value.PlayerObject.GetComponent<PlayerManager>().SpawnPlayerObject();
            }
            _alivePlayerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            SetAlivePlayerCountClientRpc(AlivePlayerCount);
        }
        _inGameUI = GameObject.Find("InGameUI").GetComponent<InGameUI>();

        State = GameState.Running;
    }
    public void PlayAgain()
    {
        if (NetworkManager.IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                client.Value.PlayerObject.GetComponent<PlayerManager>().DespawnPlayerObject();
            }
            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
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
