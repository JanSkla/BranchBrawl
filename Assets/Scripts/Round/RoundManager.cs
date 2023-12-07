using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : NetworkBehaviour
{
    private InGameUI _inGameUI;

    private GameManager _gameManager;

    private int _alivePlayerCount;
    public int AlivePlayerCount
    {
        get { return _alivePlayerCount; }
        set
        {
            _alivePlayerCount = value;
            if (_alivePlayerCount <= 1)
            {
                State = RoundState.Over;
            }
        }
    }

    private RoundState _state;
    public RoundState State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (value)
            {
                case RoundState.Running:
                    GameSetRunning();
                    return;
                case RoundState.Over:
                    GameOver();
                    return;
            }
        }
    }

    public Action GameOver;
    void Start()
    {
        GameOver += OnGameOver;
        _gameManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();

        var enumerator = _gameManager.PlayersGameData.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Debug.Log(enumerator.Current.Crowns);
        }

        StartGame();
    }
    private void GameSetRunning()
    {
        _inGameUI.Game.GetComponent<GameUI>().UpdateGameScreen(false);
    }
    private void OnGameOver()
    {
        _inGameUI.Game.GetComponent<GameUI>().UpdateGameScreen(true);

        //add crown to round winner

        if (!NetworkManager.IsServer) return;

        var enumerator = _gameManager.PlayersGameData.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Debug.Log(NetworkManager.Singleton.SpawnManager.SpawnedObjects[enumerator.Current.PMNwId].GetComponent<PlayerManager>().PlayerObject.GetComponent<Player>().IsAlive);
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects[enumerator.Current.PMNwId].GetComponent<PlayerManager>().PlayerObject.GetComponent<Player>().IsAlive)
            {
                _gameManager.PlayersGameData.Add(new PlayerGameData()
                {
                    PMNwId = enumerator.Current.PMNwId,
                    Crowns = enumerator.Current.Crowns + 1,
                });
                _gameManager.PlayersGameData.Remove(enumerator.Current);
                break;
            }
        }
        //~

        StartCoroutine(nameof(StartNewRoundCountdown));
    }
    IEnumerator StartNewRoundCountdown()
    {
        var cTxt = _inGameUI.Game.GetComponent<GameUI>().CountDownText;

        int time = 5;

        for (int i = 0; i < time; i++)
        {
            cTxt.text = (time - i).ToString();
            yield return new WaitForSeconds(1);
        }
        cTxt.text = (time - 0).ToString();
        PlayAgain();
        yield return null;
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
        _inGameUI.OnRoundStarted();

        State = RoundState.Running;
    }
    public void PlayAgain()
    {
        if (NetworkManager.IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                client.Value.PlayerObject.GetComponent<PlayerManager>().DespawnPlayerObject();
            }
            GameObject.Find("GameManager(Clone)").GetComponent<GameManager>().CurrentRoundFinished();
        }
    }

    [ClientRpc]
    private void SetAlivePlayerCountClientRpc(int alivePlayerCount)
    {
        _alivePlayerCount = alivePlayerCount;
    }
}
public enum RoundState
{
    Running,
    Over
}
