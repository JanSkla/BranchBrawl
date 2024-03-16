using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class RoundManager : NetworkBehaviour
{
    private InGameUI _inGameUI;

    [SerializeField]
    private List<Transform> _playerSpawns;

    [SerializeField]
    private GameObject _waitingForOthersScreen;

    [SerializeField]
    private NetworkCountdownText _startRoundCountdown;

    private GameManager _gameManager;

    private int _alivePlayerCount;

    private int _clientsLoaded;
    public Action AllPlayersLoaded;

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

    private RoundState _state = RoundState.PreGame;
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
        CursorHandler.Default();

        var enumerator = _gameManager.PlayersGameData.GetEnumerator();

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
            AllPlayersLoadedClientRPC();
        }
    }

    [ClientRpc]
    private void AllPlayersLoadedClientRPC()
    {
        AllPlayersLoaded.Invoke();
        StartCountdown();
    }
    public void StartCountdown()
    {
        _startRoundCountdown.StartCountDown();

        if (NetworkManager.IsServer || NetworkManager.IsHost)
        {
            for (int i = 0; i < _playerSpawns.Count; i++)
            {
                Transform tmp = _playerSpawns[i];
                int r = Random.Range(i, _playerSpawns.Count);
                _playerSpawns[i] = _playerSpawns[r];
                _playerSpawns[r] = tmp;
            }
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                client.Value.PlayerObject.GetComponent<PlayerManager>().SpawnPlayerObject(_playerSpawns[(int)client.Key % _playerSpawns.Count].position);

                if (_gameManager.RoundsList[_gameManager.CurrentRoundListIndex] == 1)
                {
                    GBase gun = client.Value.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager.GunCurrentData.Value.NetworkSpawn();

                    StartCoroutine(EquipInitItem(gun, client));
                }
                //NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerObject.AreControlsDisabled = true;
            }
            _alivePlayerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            SetAlivePlayerCountClientRpc(AlivePlayerCount);
        }
        //else
        //{
        //    NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerObject.AreControlsDisabled = true;
        //}
        _waitingForOthersScreen.SetActive(false);
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

        Debug.Log(_gameManager.PlayersGameData[0]);

        while (enumerator.MoveNext())
        {
            var player = NetworkManager.Singleton.ConnectedClients[enumerator.Current.ClientId].PlayerObject.GetComponent<PlayerManager>().PlayerObject.GetComponent<Player>();

            if (player.IsAlive)
            {
                _gameManager.PlayersGameData.Add(new PlayerGameData()
                {
                    ClientId = enumerator.Current.ClientId,
                    Crowns = enumerator.Current.Crowns + 1,
                    PlayerName = enumerator.Current.PlayerName
                });
                _gameManager.PlayersGameData.Remove(enumerator.Current);
                break;
            }

            //var pgm = player.GetComponent<PlayerGunManager>();
            //var pi = player.GetComponent<PlayerInventory>();
            //var equippedObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[pi.EquippedItem.Value.NetworkObjectId];
            //Debug.Log(equippedObject);
            //var equippedGBase = equippedObject.GetComponent<GBase>();
            //if (equippedGBase)
            //{
            //    pgm.GunCurrentData.Value = new(equippedGBase);
            //}
        }
        //~

        _inGameUI.Game.GetComponent<GameUI>().CountDownText.StartCountDown();
    }
    public void StartGame()
    {
        _inGameUI = GameObject.Find("InGameUI").GetComponent<InGameUI>();
        //_inGameUI.OnRoundStarted();

        State = RoundState.Running;

        //enable movement
        var po = NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerObject;
        po.AreControlsDisabled = false;
        Utils.ChangeLayerWithChildren(po.gameObject, LayerMask.NameToLayer("LocalPlayer"));
        _inGameUI.AllowControlsWhenExitingMenu = true;
    }

    IEnumerator EquipInitItem(GBase gun, KeyValuePair<ulong, NetworkClient> client)
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
            if(_gameManager.CurrentRound == RoundType.FirstCombat)
            {
                var enumerator = _gameManager.PlayersGameData.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var client = NetworkManager.Singleton.ConnectedClients[enumerator.Current.ClientId].PlayerObject;
                    var player = client.GetComponent<PlayerManager>().PlayerObject.GetComponent<Player>();

                    var pgm = client.GetComponent<PlayerManager>().PlayerGunManager;
                    var pi = player.GetComponent<PlayerInventory>();
                    if (pi.EquippedItem.Value.NetworkObjectId == 0) continue;
                    var equippedObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[pi.EquippedItem.Value.NetworkObjectId];
                    Debug.Log(equippedObject);
                    var equippedGBase = equippedObject.GetComponent<GBase>();
                    if (equippedGBase)
                    {
                        Debug.Log(pgm);
                        pgm.GunCurrentData.Value = new(equippedGBase);

                        var gunString = GunBaseSaveData.ParseToText(pgm.GunCurrentData.Value.Child);

                        for (int i = 0; i < gunString.Length; i++)
                        {
                            if (char.IsNumber(gunString[i]))
                            {
                                AddUsedUpgradeClientRpc(gunString[i] - '0', pgm.NetworkObjectId);
                            }
                        }
                    }

                    //client.GetComponent<PlayerManager>().DespawnPlayerObject();
                }
            }
            GameObject.Find("GameManager(Clone)").GetComponent<GameManager>().CurrentRoundFinished();
        }
    }

    [ClientRpc]
    private void AddUsedUpgradeClientRpc(int upgrade, ulong pgmNwId)
    {
        var pgm = NetworkManager.Singleton.SpawnManager.SpawnedObjects[pgmNwId];

        pgm.GetComponent<PlayerGunManager>().AddGUpgrade(upgrade, true);
    }

    [ClientRpc]
    private void SetAlivePlayerCountClientRpc(int alivePlayerCount)
    {
        _alivePlayerCount = alivePlayerCount;
    }
}
public enum RoundState
{
    PreGame,
    Running,
    Over
}
