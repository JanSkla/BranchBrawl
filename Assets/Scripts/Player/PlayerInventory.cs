using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public NetworkVariable<Item> EquippedItem = new ();

    private Player player;
    private GameManager _gameManager;

    public static Item _emptyItem = new ();
    void Start()
    {
        player = GetComponent<Player>();
        _gameManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();

        if (NetworkManager.IsClient) //equip item on load
        {
            if (EquippedItem.Value.Equals(_emptyItem)) return;

            GameObject go = GetNetworkObject(EquippedItem.Value.NetworkObjectId).gameObject;

            if (go != null)
            {
                if(go.GetComponent<Rigidbody>())
                    go.GetComponent<Rigidbody>().isKinematic = false;
                //go.transform.SetParent(transform);
                if (go.GetComponent<NetworkTransform>())
                    go.GetComponent<NetworkTransform>().enabled = false;
            }
        }
    }
    void Update()
    {

        if (_gameManager.RoundsList[_gameManager.CurrentRoundListIndex] == 0 && player.IsLocalPlayer && !player.AreControlsDisabled)
        {
            if (Input.GetKeyDown(KeyCode.E) && EquippedItem.Value.Equals(_emptyItem))
            {
                GameObject pickableObject = player.GetComponent<PlayerCamera>().GetFacingPickable();
                if (pickableObject != null)
                {
                    if (pickableObject.CompareTag("Stick"))
                    {
                        Vector3 stickPartLocalPos = pickableObject.transform.localPosition;
                        //while (pickableObject.transform.parent != null)
                        //{
                        //    pickableObject = pickableObject.transform.parent.gameObject;
                        //}

                        EquipItem(new Item()
                        {
                            Id = 0,
                            NetworkObjectId = pickableObject.GetComponent<NetworkObject>().NetworkObjectId,
                            PositionOffset = stickPartLocalPos,
                        }
                        );
                    }
                    else
                    {
                        //while (pickableObject.transform.parent != null)
                        //{
                        //    pickableObject = pickableObject.transform.parent.gameObject;
                        //}

                        EquipItem(new Item()
                        {
                            Id = 0,
                            NetworkObjectId = pickableObject.GetComponent<NetworkObject>().NetworkObjectId,
                        });
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Q) && !EquippedItem.Value.Equals(_emptyItem))
            {
                UnequipItem();
            }
        }
    }

    public void EquipItem(Item itemToEquip)
    {
        if (NetworkManager.IsServer)
        {
            if (!EquippedItem.Value.Equals(_emptyItem))
            {
                UnequipItem();
            }

            EquippedItem.Value = itemToEquip;

            GameObject equipGO = GetNetworkObject(EquippedItem.Value.NetworkObjectId).gameObject;

            while (equipGO.transform.parent != null)
            {
                equipGO = equipGO.transform.parent.gameObject; //TADY POKRACUJ ZITRA
            }

            Transform handTransform = player.Hand.transform;
            equipGO.GetComponent<NetworkObject>().TrySetParent(handTransform.gameObject, false).ToString();

            SharedServerClientEquipActions(equipGO, itemToEquip);

            if (equipGO.CompareTag("Stick"))
            {
                Stick.FindGunBarrels(GetNetworkObject(EquippedItem.Value.NetworkObjectId).gameObject.GetComponent<StickPart>());
            }
        }
        else
        {
            EquipItemServerRpc(itemToEquip);
        }
    }

    public void UnequipItem()
    {
        if (NetworkManager.IsServer)
        {
            GameObject unequippedGO = GetNetworkObject(EquippedItem.Value.NetworkObjectId).gameObject;

            unequippedGO.GetComponent<NetworkObject>().TryRemoveParent();
            EquippedItem.Value = _emptyItem;

            int changeLayer = LayerMask.NameToLayer("Pickable");

            unequippedGO.layer = changeLayer;
            foreach (Transform child in unequippedGO.transform)
            {
                child.gameObject.layer = changeLayer;
            }
            SharedServerClientUnequipActions(unequippedGO);
        }
        else
        {
            UnequipItemServerRpc();
        }
    }

    public override void OnNetworkSpawn()
    {
        EquippedItem.OnValueChanged += OnEquippedItemChange;
    }

    public override void OnNetworkDespawn()
    {
        EquippedItem.OnValueChanged -= OnEquippedItemChange;
    }

    private void OnEquippedItemChange(Item previousItem, Item newItem)
    {
        if (!newItem.Equals(_emptyItem))
        {
            player.SetHandInPosition();
        }
        else
        {
            player.SetHandInOffPosition();
        }

        if (NetworkManager.IsServer) return;

        if (!newItem.Equals(_emptyItem))
        {
            GameObject n = GetNetworkObject(newItem.NetworkObjectId).gameObject;

            if (n.CompareTag("Stick") && n.transform.parent.gameObject)
            {
                n = n.transform.parent.gameObject;
            }

            if (n.GetComponent<NetworkTransform>())
                n.GetComponent<NetworkTransform>().enabled = false;

            SharedServerClientEquipActions(n, newItem);

        }
        else
        {
            GameObject prevGO = GetNetworkObject(previousItem.NetworkObjectId).gameObject;

            if (prevGO.GetComponent<NetworkTransform>())
                prevGO.GetComponent<NetworkTransform>().enabled = true;

            SharedServerClientUnequipActions(prevGO);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void EquipItemServerRpc(Item itemToEquip)
    {
        EquipItem(itemToEquip);
    }
    [ServerRpc(RequireOwnership = false)]
    private void UnequipItemServerRpc()
    {
        UnequipItem();
    }

    //utils
    private void SharedServerClientEquipActions(GameObject equipGO, Item itemToEquip)
    {
        equipGO.transform.localPosition = new Vector3();
        equipGO.transform.localRotation = new Quaternion();

        if(equipGO.GetComponent<Rigidbody>())
            equipGO.GetComponent<Rigidbody>().isKinematic = true;

        if (equipGO.CompareTag("Gun"))
        {
            player.GetComponent<PlayerShoot>().shootInput += equipGO.GetComponent<Gun>().Shoot;
        }
        else if (equipGO.GetComponent<GBase>())
        {
            player.GetComponent<PlayerShoot>().shootInput += equipGO.GetComponent<GBase>().Shoot;
            Debug.Log("Shot assigned");
        }

        int changeLayer = player.IsLocalPlayer ? LayerMask.NameToLayer("LocalPlayer") : LayerMask.NameToLayer("Player");


        Tools.ChangeLayerWithChildren(equipGO, changeLayer);
        //equipGO.layer = changeLayer;
        //foreach (Transform child in equipGO.transform)
        //{
        //    child.gameObject.layer = changeLayer;
        //    if (itemToEquip.PositionOffset != null)
        //    {
        //        child.localPosition -= itemToEquip.PositionOffset;
        //    }
        //}
    }

    private void SharedServerClientUnequipActions(GameObject unequippedGO)
    {
        if (unequippedGO.CompareTag("Gun"))
        {
            player.GetComponent<PlayerShoot>().shootInput -= unequippedGO.GetComponent<Gun>().Shoot;
        }

        if (unequippedGO.CompareTag("Stick") && unequippedGO.transform.parent.gameObject)
        {
            unequippedGO = unequippedGO.transform.parent.gameObject;
        }

        int changeLayer = LayerMask.NameToLayer("Pickable");

        Tools.ChangeLayerWithChildren(unequippedGO, changeLayer);

        unequippedGO.GetComponent<Rigidbody>().isKinematic = false;
    }
}
