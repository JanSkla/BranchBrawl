using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkData : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> JoinCode;
    public NetworkList<ulong> PlayerObjectNwIds = new();

    [SerializeField]
    private GameObject _gameManagerPrefab;

    private GameObject _gameManager;

    public void StartNewGame()
    {
        if (_gameManager)
        {
            _gameManager.GetComponent<NetworkObject>().Despawn();
            Destroy(_gameManager);
        }
        _gameManager = Instantiate(_gameManagerPrefab);
    }
}
