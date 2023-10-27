using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    public GameObject PlayerObject;
    private NetworkVariable<ulong> _playerObjectNwId = new();

    void Start()
    {
        //if (IsClient)
        //{
        //    PlayerObject = NetworkManager.SpawnManager.SpawnedObjects[_playerObjectNwId.Value].gameObject;
        //}
    }
    public override void OnNetworkSpawn()
    {
        _playerObjectNwId.OnValueChanged += OnPlayerObjectNwIdChange;
        //if (IsServer)
        //{
        //    SpawnPlayerObject();
        //}
    }

    public void SpawnPlayerObject()
    {
        PlayerObject = Instantiate(playerPrefab);
        PlayerObject.GetComponent<Player>().PlayerManager = GetComponent<NetworkObject>();
        PlayerObject.GetComponent<NetworkObject>().Spawn();
        _playerObjectNwId.Value = PlayerObject.GetComponent<NetworkObject>().NetworkObjectId;
        //_player.GetComponent<NetworkObject>().TrySetParent(transform);
    }
    public void DespawnPlayerObject()
    {
        PlayerObject.GetComponent<NetworkObject>().Despawn();
    }

    private void OnPlayerObjectNwIdChange(ulong prevId, ulong newId)
    {
        PlayerObject = NetworkManager.SpawnManager.SpawnedObjects[newId].gameObject;
    }
}
