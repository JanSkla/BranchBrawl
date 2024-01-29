using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStandPlacehodlerManager : MonoBehaviour
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

        int j = 1;
        for (int i = 0; i < playerNwIds.Length; i++)
        {

            var go = Instantiate(_playerstandPrefab.gameObject);
            go.transform.SetParent(transform, false);


            var playerManager = NetworkManager.Singleton.SpawnManager.SpawnedObjects[_networkData.PlayerObjectNwIds[i]].GetComponent<PlayerManager>();


            go.GetComponent<PlayerStand>().SetNickname(playerManager.PlayerName.Value.ToString());
            Vector3 pos;
            if (playerManager.IsLocalPlayer)
            {
                go.GetComponent<PlayerStand>().SetNameVisibility(false);

                pos = new(0, -1, 0);
            }
            else
            {
                j++;
                int row = (int)Mathf.Ceil((float)j / 2);
                int sideLROffset = (int)Mathf.Pow(-1, j) * row;

                pos = new(sideLROffset * 5, -1, row * 4);
                if (_readyButton.ReadyList.Contains((ulong)i))
                {
                    go.GetComponent<PlayerStand>().SetReady(true);
                }
            }
            go.transform.localPosition = pos;
            _playerStands.Add((ulong)i, go.GetComponent<PlayerStand>());
        }
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
