using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkSuccessBtn : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _totalText;

    [SerializeField]
    private UnityEvent _onFullfilled;

    private int _playerCount;
    private bool _isReady = false;

    private NetworkList<ulong> _readyList = new NetworkList<ulong>();

    private void Start()
    {
        _totalText.gameObject.SetActive(false);
        if (_onFullfilled.GetPersistentEventCount() == 0)
        {
            Debug.LogWarning("No OnFullfill event assigned");
        }
    }
    void OnEnable()
    {
        if (NetworkManager.IsServer)
        {
            _playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        }
    }

    public override void OnNetworkSpawn()
    {
        _readyList.OnListChanged += OnReadyListChange;


        if (NetworkManager.IsServer || NetworkManager.IsHost)
        {
            _playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        }
        else
        {
            FetchPlayerCountServerRpc();
        }
    }
    public override void OnNetworkDespawn()
    {
        _readyList.OnListChanged -= OnReadyListChange;
    }

    public void OnButtonPress()
    {
        if (NetworkManager.IsServer || NetworkManager.IsHost)
        {
            _isReady = !_isReady;
            if (_isReady)
            {
                _readyList.Add(NetworkManager.Singleton.LocalClientId);
            }
            else
            {
                _readyList.Remove(NetworkManager.Singleton.LocalClientId);
            }
            UpdateText(_readyList.Count);
        }
        else
        {
            _isReady = !_isReady;
            UpdateText(_readyList.Count + (_isReady ? 1 : -1)); //simulated count increase
            IsReadyServerRPC(_isReady, NetworkManager.Singleton.LocalClientId);
        }
    }

    private void UpdateText(int readyCount)
    {
        _totalText.text = $"{readyCount}/{_playerCount}";
    }

    private void OnReadyListChange(NetworkListEvent<ulong> changeEvent)
    {
        if (!_totalText.gameObject.activeSelf)
        {
            _totalText.gameObject.SetActive(true);
        }
        UpdateText(_readyList.Count);
        if (NetworkManager.IsServer && _readyList.Count == _playerCount)
        {
            Fullfilled();
        }
    }

    private void Fullfilled()
    {
        _onFullfilled.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    private void IsReadyServerRPC(bool isReady, ulong clientId)
    {
        if (isReady)
        {
            _readyList.Add(clientId);
        }
        else
        {
            _readyList.Remove(clientId);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void FetchPlayerCountServerRpc()
    {
        SetPlayerCountClientRpc(NetworkManager.Singleton.ConnectedClientsIds.Count);
    }
    [ClientRpc]
    private void SetPlayerCountClientRpc(int playerCount)
    {
        _playerCount = playerCount;
    }
}
