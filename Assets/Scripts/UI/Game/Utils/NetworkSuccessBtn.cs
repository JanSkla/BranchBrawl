using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkSuccessBtn : NetworkBehaviour
{
    public Action Fulfilled;

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private GameManager gameManager;

    private int _playerCount;
    private bool _isReady = false;

    private NetworkVariable<int> _readyCount = new NetworkVariable<int>();

    void OnEnable()
    {
        if (IsServer)
        {
            _playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        }
    }

    public override void OnNetworkSpawn()
    {
        _readyCount.OnValueChanged += OnReadyCountChange;


        if (NetworkManager.IsServer || NetworkManager.IsHost)
        {
            _readyCount.Value = 0;
            _playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        }
        else
        {
            FetchPlayerCountServerRpc();
        }
    }
    public override void OnNetworkDespawn()
    {
        _readyCount.OnValueChanged -= OnReadyCountChange;
    }

    public void OnButtonPress()
    {
        if (NetworkManager.IsServer || NetworkManager.IsHost)
        {
            _isReady = !_isReady;
            if (_isReady)
            {
                _readyCount.Value++;
            }
            else
            {
                _readyCount.Value--;
            }
            UpdateText(_readyCount.Value);
        }
        else
        {
            _isReady = !_isReady;
            UpdateText(_readyCount.Value + (_isReady ? 1 : -1)); //simulated count increase
            IsReadyServerRPC(_isReady);
        }
    }

    private void UpdateText(int readyCount)
    {
        text.text = $"Play again {readyCount}/{_playerCount}";
    }

    private void OnReadyCountChange(int prevCount, int newCount)
    {
        UpdateText(newCount);
        if (NetworkManager.IsServer && newCount == _playerCount)
        {
            Fulfilled.Invoke();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void IsReadyServerRPC(bool isReady)
    {
        if (isReady)
        {
            _readyCount.Value++;
        }
        else
        {
            _readyCount.Value--;
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