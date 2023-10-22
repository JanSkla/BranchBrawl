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

    public override void OnNetworkSpawn()
    {
        _readyCount.OnValueChanged += OnReadyCountChange;


        if (NetworkManager.IsServer || NetworkManager.IsHost)
        {
            _readyCount.Value = 0;
            _playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            UpdateText(_readyCount.Value);
        }
        else
        {
            Debug.Log(1);
            FetchPlayerCountServerRpc();
        }
    }
    public override void OnNetworkDespawn()
    {
        _readyCount.OnValueChanged -= OnReadyCountChange;
    }

    public void OnButtonPress()
    {
        Debug.Log(1);
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
            Debug.Log(1);
            _isReady = !_isReady;
            UpdateText(_readyCount.Value + (_isReady ? 1 : -1)); //simulated count increase
            IsReadyServerRPC(_isReady);
        }
    }

    private void UpdateText(int readyCount)
    {
        Debug.Log(12);
        text.text = $"Play again {readyCount}/{_playerCount}";
    }

    private void OnReadyCountChange(int prevCount, int newCount)
    {
        Debug.Log(2);
        UpdateText(newCount);
        if (newCount == _playerCount && IsServer)
        {
            Fulfilled.Invoke();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void IsReadyServerRPC(bool isReady)
    {
        Debug.Log(1);
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
        Debug.Log(1);
        SetPlayerCountClientRpc(NetworkManager.Singleton.ConnectedClientsIds.Count);
    }
    [ClientRpc]
    private void SetPlayerCountClientRpc(int playerCount)
    {
        Debug.Log(1);
        _playerCount = playerCount;
        UpdateText(_readyCount.Value);
    }
}
