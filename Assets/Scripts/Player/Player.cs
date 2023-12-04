
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private GameObject handPrefab;
    [SerializeField]
    public GameObject Head;
    [SerializeField]
    private TextMeshPro _nameTag;

    public GameObject Hand;
    private NetworkVariable<ulong> _handNwId = new();

    public NetworkObject PlayerManager;
    private NetworkVariable<ulong> _playerManagerNwId = new();

    private bool _isAlive = true;
    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;

            if (!_isAlive)
            {
                Die();
            }
            if (IsLocalPlayer)
            {
                if(GetComponent<LocalPlayer>().InGameUI != null)
                    GetComponent<LocalPlayer>().InGameUI.DeathScreen(_isAlive);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.IsServer)
        {
            _playerManagerNwId.Value = PlayerManager.NetworkObjectId; //IsLocalPlayer works after
        }
        else if (NetworkManager.IsClient)
        {

            PlayerManager = NetworkManager.SpawnManager.SpawnedObjects[_playerManagerNwId.Value].gameObject.GetComponent<NetworkObject>();

            Hand = NetworkManager.SpawnManager.SpawnedObjects[_handNwId.Value].gameObject;
            Hand.layer = IsLocalPlayer ? 8 : 6;
        }
        if (IsLocalPlayer)
        {
            name = "Local Player";
            GetComponent<LocalPlayer>().enabled = true;
        }
        if (NetworkManager.IsServer)
        {
            Hand = Instantiate(handPrefab);
            Hand.layer = IsLocalPlayer ? 8 : 6;
            Hand.GetComponent<NetworkObject>().Spawn();
            _handNwId.Value = Hand.GetComponent<NetworkObject>().NetworkObjectId;
            Hand.GetComponent<NetworkObject>().TrySetParent(transform, false);
        }
        _nameTag.text = PlayerManager.gameObject.GetComponent<PlayerManager>().PlayerName.Value.ToString();
    }

    //public override void OnNetworkDespawn()
    //{
    //    if (NetworkManager.IsClient)
    //    {
    //        _playerManagerNwId.OnValueChanged -= OnPlayerManagerNwIdChange;
    //        _handNwId.OnValueChanged -= OnHandNwIdChange;
    //    }
    //}

    public void Die()
    {
        RoundManager roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
        if (roundManager)
        {
            int placement = roundManager.AlivePlayerCount;
            roundManager.AlivePlayerCount--;

            if (IsLocalPlayer)
            {
                GameObject.Find("InGameUI").GetComponent<InGameUI>().PlacementText.text = placement.ToString();
            }
        }
        GetComponent<Renderer>().material.color = Color.red;
    }

    public new bool IsLocalPlayer
    {
        get { return PlayerManager.IsLocalPlayer; }
    }



    //private void OnPlayerManagerNwIdChange(ulong prevId, ulong newId)
    //{
    //    Debug.Log("PM");
    //    PlayerManager = NetworkManager.SpawnManager.SpawnedObjects[newId].gameObject.GetComponent<NetworkObject>();
    //}
    //private void OnHandNwIdChange(ulong prevId, ulong newId)
    //{
    //    Debug.Log("H");
    //    Hand = NetworkManager.SpawnManager.SpawnedObjects[newId].gameObject;
    //    Hand.layer = IsLocalPlayer ? 8 : 6;
    //}
}
