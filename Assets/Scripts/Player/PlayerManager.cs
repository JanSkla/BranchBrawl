using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PlayerManager : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> PlayerName;

    private string[] gamerTags = {
        "ShadowStrike",
        "LunarPhoenix",
        "CyberPulse",
        "EternalSpecter",
        "QuantumNinja",
        "MysticVortex",
        "RoguePixel",
        "NeonThunder",
        "AstroBlaze",
        "ViperByte"
    };

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
        if (IsServer)
        {
            PlayerName.Value = gamerTags[NetworkManager.LocalClientId % (ulong)gamerTags.Length] + NetworkManager.LocalClientId;
        }
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
        PlayerObject.GetComponent<Player>().Hand.GetComponent<NetworkObject>().Despawn();
        PlayerObject.GetComponent<NetworkObject>().Despawn();
    }

    private void OnPlayerObjectNwIdChange(ulong prevId, ulong newId)
    {
        PlayerObject = NetworkManager.SpawnManager.SpawnedObjects[newId].gameObject;
    }
}
