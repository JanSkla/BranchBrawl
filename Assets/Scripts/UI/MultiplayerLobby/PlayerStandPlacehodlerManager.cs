using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStandPlacehodlerManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerStand _playerstandPrefab;

    [SerializeField]
    private NetworkSuccessBtn _readyButton;

    private NetworkData _networkData;

    private Dictionary<ulong, PlayerStand> _playerStands = new Dictionary<ulong, PlayerStand>();

    private void Start()
    {
        _readyButton.ReadyList.OnListChanged += OnListChange;
    }

    public void RerenderCurrentPlayers(ulong[] playerNwIds)
    {
        foreach (Transform child in transform)
        {
            Tools.DestroyWithChildren(child.gameObject);
            _playerStands.Clear();
        }

        if(_networkData == null)
            _networkData = GameObject.Find("NetworkDataManager(Clone)").GetComponent<NetworkData>();

        for (int i = 0; i < playerNwIds.Length; i++)
        {
            int row = (int)Mathf.Ceil((float)i / 2);
            int sideLROffset = (int)Mathf.Pow(-1, i) * row;

            Vector3 pos = new(sideLROffset * 5, -1, row * 4);

            var go = Instantiate(_playerstandPrefab.gameObject);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = pos;

            Debug.Log(_networkData);
            foreach (var a in _networkData.PlayerObjectNwIds)
            {
                Debug.Log(a);
            }
            Debug.Log(_networkData.PlayerObjectNwIds[i]);
            Debug.Log(NetworkManager.Singleton.SpawnManager.SpawnedObjects[_networkData.PlayerObjectNwIds[i]]);
            Debug.Log(NetworkManager.Singleton.SpawnManager.SpawnedObjects[_networkData.PlayerObjectNwIds[i]].GetComponent<PlayerManager>());
            Debug.Log(NetworkManager.Singleton.SpawnManager.SpawnedObjects[_networkData.PlayerObjectNwIds[i]].GetComponent<PlayerManager>());

            var playerManager = NetworkManager.Singleton.SpawnManager.SpawnedObjects[_networkData.PlayerObjectNwIds[i]].GetComponent<PlayerManager>();


            go.GetComponent<PlayerStand>().SetNickname(playerManager.PlayerName.Value.ToString());

            if (playerManager.IsLocalPlayer)
            {
                go.GetComponent<PlayerStand>().SetNameVisibility(false);
            }
            else
            {
                if (_readyButton.ReadyList.Contains((ulong)i))
                {
                    go.GetComponent<PlayerStand>().SetReady(true);
                }
            }
            _playerStands.Add((ulong)i, go.GetComponent<PlayerStand>());
        }
    }
    [ServerRpc]
    private void RerenderStandsServerRpc()
    {

    }
    private void OnListChange(NetworkListEvent<ulong> changeEvent)
    {
        Debug.Log("a");
        for (int i = 0; i < _playerStands.Count; i++)
        {
            if (_readyButton.ReadyList.Contains((ulong)i))
            {
                _playerStands[(ulong)i].SetReady(true);
            }
            else
            {
                _playerStands[(ulong)i].SetReady(false);
            }
        }
    }
}
