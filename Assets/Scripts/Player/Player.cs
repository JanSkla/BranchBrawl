
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class Player : NetworkBehaviour
{

    [SerializeField]
    private GameObject handPrefab;
    [SerializeField]
    public GameObject Head;
    [SerializeField]
    private TextMeshPro _nameTag;
    [SerializeField]
    private TwoBoneIKConstraint _handIKConstraint;

    [SerializeField]
    private RigBuilder _rigBuilder;
    [SerializeField]
    public Animator RigAnimator;

    public GameObject Hand;
    private NetworkVariable<ulong> _handNwId = new();

    public PlayerManager PlayerManager;
    private NetworkVariable<ulong> _playerManagerNwId = new();

    private bool _isAlive = true;
    //movement restrictors
    public bool AreControlsDisabled = true;
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
                    GetComponent<LocalPlayer>().InGameUI.Game.GetComponent<GameUI>().DeathScreen(_isAlive);
            }
        }
    }
    //private void Update()
    //{
    //    Physics.Raycast(GetComponent<PlayerCamera>().FpsCam.transform.position, Vector3.forward, out var hit, Mathf.Infinity);

    //    Debug.Log("cam "+GetComponent<PlayerCamera>().FpsCam.transform.position);
    //    Debug.Log(hit.point);

    //    Debug.DrawLine(Hand.transform.position, hit.point, Color.cyan);

    //    Debug.Log(hit.point);

    //    var hitPoint = Hand.transform.InverseTransformPoint(hit.point);
    //    Hand.transform.localRotation = Quaternion.LookRotation(hitPoint.normalized);
    //}
    public override void OnNetworkSpawn()
    {
        var igui = GameObject.Find("InGameUI");
        if (igui.IsUnityNull())
        {
            Debug.LogWarning("InGameUI not found in the scene, something is wrong!");
        }

        gameObject.layer = 6;
        if (NetworkManager.IsServer)
        {
            _playerManagerNwId.Value = PlayerManager.NetworkObjectId; //IsLocalPlayer works after
        }
        else if (NetworkManager.IsClient)
        {

            PlayerManager = NetworkManager.SpawnManager.SpawnedObjects[_playerManagerNwId.Value].gameObject.GetComponent<PlayerManager>();

            Hand = NetworkManager.SpawnManager.SpawnedObjects[_handNwId.Value].gameObject;

            Utils.ChangeLayerWithChildren(Hand, IsLocalPlayer ? 8 : 6);

            PlayerManager.PlayerObject = this;
        }
        if (IsLocalPlayer)
        {
            name = "Local Player";
            GetComponent<LocalPlayer>().enabled = true;
            _nameTag.enabled = false;
            GetComponent<LocalPlayer>().InGameUI = igui.GetComponent<InGameUI>();
            igui.GetComponent<InGameUI>().CurrentPlayer = this;
        }
        if (NetworkManager.IsServer)
        {
            Hand = Instantiate(handPrefab);

            Utils.ChangeLayerWithChildren(Hand, IsLocalPlayer ? 8 : 6);

            Hand.GetComponent<NetworkObject>().Spawn();
            _handNwId.Value = Hand.GetComponent<NetworkObject>().NetworkObjectId;
            Hand.GetComponent<NetworkObject>().TrySetParent(transform, false);

        }
        _nameTag.text = PlayerManager.gameObject.GetComponent<PlayerManager>().PlayerName.Value.ToString();
        Utils.ChangeLayerWithChildren(gameObject, IsLocalPlayer ? 8 : 6);
    }

    //public override void OnNetworkDespawn()
    //{
    //    if (NetworkManager.IsClient)
    //    {
    //        _playerManagerNwId.OnValueChanged -= OnPlayerManagerNwIdChange;
    //        _handNwId.OnValueChanged -= OnHandNwIdChange;
    //    }
    //}
    public void SetHandInOffPosition()
    {
        RigAnimator.SetBool("RHandHold", false);
        _handIKConstraint.data.target = null;
        _rigBuilder.Build();
    }
    public void SetHandInPosition()
    {
        RigAnimator.SetBool("RHandHold", true);
        _handIKConstraint.data.target = Hand.transform.GetChild(0);
        _rigBuilder.Build();
    }
    public void Die()
    {
        RoundManager roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
        if (roundManager)
        {
            int placement = roundManager.AlivePlayerCount;
            roundManager.AlivePlayerCount--;

            if (IsLocalPlayer)
            {
                GameObject.Find("InGameUI").GetComponent<InGameUI>().Game.GetComponent<GameUI>().PlacementText.text = placement.ToString();
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
