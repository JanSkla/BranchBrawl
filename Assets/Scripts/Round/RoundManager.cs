using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : NetworkBehaviour
{
    private InGameUI _inGameUI;

    [SerializeField]
    private GameObject _waitingForOthersScreen;

    private GameManager _gameManager;

    private int _alivePlayerCount;

    private int _clientsLoaded;
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

        if (IsClient)
        {
            SceneLoadedServerRPC();
        }

    }
    [ServerRpc(RequireOwnership = false)]
    private void SceneLoadedServerRPC()
    {
        _clientsLoaded++;
        if (_clientsLoaded >= NetworkManager.Singleton.ConnectedClients.Count)
        {
            Debug.Log(_clientsLoaded);
            EveryoneLoadedClientRPC();
        }
    }

    [ClientRpc]
    private void EveryoneLoadedClientRPC()
    {
        Debug.Log("bre");
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

        _inGameUI.Game.GetComponent<GameUI>().CountDownText.StartCountDown();
    }
    private void StartGame()
    {
        if (NetworkManager.IsServer || NetworkManager.IsHost)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                client.Value.PlayerObject.GetComponent<PlayerManager>().SpawnPlayerObject();

                if (_gameManager.RoundsList[_gameManager.CurrentRoundListIndex] == 1)
                {
                    GBase gun = client.Value.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager.GunCurrentData.Value.NetworkSpawn();

                    StartCoroutine(EquipInitItem(gun, client));
                }
            }
            _alivePlayerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            SetAlivePlayerCountClientRpc(AlivePlayerCount);
        }
        _waitingForOthersScreen.SetActive(false);
        _inGameUI = GameObject.Find("InGameUI").GetComponent<InGameUI>();
        //_inGameUI.OnRoundStarted();

        State = RoundState.Running;
    }

    IEnumerator EquipInitItem(GBase gun, System.Collections.Generic.KeyValuePair<ulong, NetworkClient> client)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(1);


        client.Value.PlayerObject.GetComponent<PlayerManager>().PlayerObject.GetComponent<PlayerInventory>().EquipItem(new Item()
        {
            Id = 0,
            NetworkObjectId = gun.GetComponent<NetworkObject>().NetworkObjectId
        });

        //Do the action after the delay time has finished.
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
