using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public NetworkVariable<Item> EquippedItem = new NetworkVariable<Item>();

    private Player player;

    public static Item _emptyItem = new Item();
    void Start()
    {
        player = GetComponent<Player>();
        if (IsClient) //equip item on load
        {
            if (EquippedItem.Value.Equals(_emptyItem)) return;

            GameObject go = GetNetworkObject(EquippedItem.Value.NetworkObjectId).gameObject;

            if (go != null)
            {
                go.GetComponent<Rigidbody>().isKinematic = false;
                //go.transform.SetParent(transform);
                go.GetComponent<NetworkTransform>().enabled = false;
            }
        }
    }
    void Update()
    {
        if (IsLocalPlayer && Input.GetKeyDown(KeyCode.E) && EquippedItem.Value.Equals(_emptyItem))
        {
            GameObject pickableObject = player.head.GetComponent<PlayerCamera>().GetFacingPickable();
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
        if (IsLocalPlayer && Input.GetKeyDown(KeyCode.Q) && !EquippedItem.Value.Equals(_emptyItem))
        {
            UnequipItem();
        }
    }

    public void EquipItem(Item itemToEquip)
    {
        if (IsServer)
        {
            EquippedItem.Value = itemToEquip;

            GameObject equipGO = GetNetworkObject(EquippedItem.Value.NetworkObjectId).gameObject;

            Transform handTransform = player.hand.transform;

            while (equipGO.transform.parent != null)
            {
                equipGO = equipGO.transform.parent.gameObject;
            }

            equipGO.GetComponent<NetworkObject>().TrySetParent(handTransform).ToString();

            int changeLayer = IsLocalPlayer ? LayerMask.NameToLayer("LocalPlayer") : LayerMask.NameToLayer("Player");

            equipGO.layer = changeLayer;
            foreach (Transform child in equipGO.transform)
            {
                child.gameObject.layer = changeLayer;
                if (itemToEquip.PositionOffset != null)
                {
                    child.localPosition = child.localPosition - itemToEquip.PositionOffset;
                }
            }

            equipGO.transform.localPosition = new Vector3();
            equipGO.transform.localRotation = new Quaternion();

            //equipGO.transform.rotation = handTransform.rotation;
            equipGO.GetComponent<Rigidbody>().isKinematic = true;

            if (equipGO.CompareTag("Stick"))
            {
                Stick.FindGunBarrels(GetNetworkObject(EquippedItem.Value.NetworkObjectId).gameObject.GetComponent<StickPart>());
            }
            else if (equipGO.CompareTag("Gun"))
            {
                player.GetComponent<PlayerShoot>().shootInput += equipGO.GetComponent<Gun>().Shoot;
            }
        }
        else
        {
            EquipItemServerRpc(itemToEquip);
        }
    }

    public void UnequipItem()
    {
        if (IsServer)
        {
            GameObject unequippedGO = GetNetworkObject(EquippedItem.Value.NetworkObjectId).gameObject;

            if (unequippedGO.CompareTag("Stick") && unequippedGO.transform.parent.gameObject)
            {
                unequippedGO = unequippedGO.transform.parent.gameObject;
            }

            if (unequippedGO.CompareTag("Gun"))
            {
                player.GetComponent<PlayerShoot>().shootInput -= unequippedGO.GetComponent<Gun>().Shoot;
            }

            unequippedGO.GetComponent<NetworkObject>().TryRemoveParent();
            EquippedItem.Value = _emptyItem;

            int changeLayer = LayerMask.NameToLayer("Pickable");

            unequippedGO.layer = changeLayer;
            foreach (Transform child in unequippedGO.transform)
            {
                child.gameObject.layer = changeLayer;
            }

            unequippedGO.GetComponent<Rigidbody>().isKinematic = false;
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
        if (IsServer) return;

        if (!newItem.Equals(_emptyItem))
        {
            GameObject n = GetNetworkObject(newItem.NetworkObjectId).gameObject;
            Transform handTransform = player.hand.transform;

            if (n.CompareTag("Stick") && n.transform.parent.gameObject)
            {
                n = n.transform.parent.gameObject;
            }

            n.GetComponent<NetworkTransform>().enabled = false;
            n.transform.transform.localPosition = new Vector3(0, 0, 0);
            n.transform.transform.rotation = handTransform.rotation;
            //n.transform.transform.localPosition += new Vector3(0.5f, 0, 0);
            //n.transform.SetParent(handTransform);
            n.GetComponent<Rigidbody>().isKinematic = true;

            int changeLayer = IsLocalPlayer ? 8 : 6;

            foreach (Transform child in n.gameObject.transform)
            {
                child.gameObject.layer = changeLayer;
                child.localPosition = child.localPosition - newItem.PositionOffset;
            }

        }
        else
        {
            GameObject prev = GetNetworkObject(previousItem.NetworkObjectId).gameObject;

            if (prev.CompareTag("Stick") && prev.transform.parent.gameObject)
            {
                prev = prev.transform.parent.gameObject;
            }

            //prev.transform.SetParent(null);
            prev.GetComponent<NetworkTransform>().enabled = true;
            prev.GetComponent<Rigidbody>().isKinematic = false;

            foreach (Transform child in prev.GameObject().transform)
            {
                child.gameObject.layer = 7;
            }
        }
    }

    [ServerRpc]
    private void EquipItemServerRpc(Item itemToEquip)
    {
        EquipItem(itemToEquip);
    }
    [ServerRpc]
    private void UnequipItemServerRpc()
    {
        UnequipItem();
    }
}
