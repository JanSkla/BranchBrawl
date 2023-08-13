using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    private Item[] _inventory;
    private NetworkVariable<Item> _equippedItem = new NetworkVariable<Item>();

    [SerializeField]
    private GameObject StickPrefab;

    private Item _emptyItem = new Item();
    void Start()
    {
        if (IsServer)
        {
            GameObject go = Instantiate(StickPrefab);
            go.GetComponent<NetworkObject>().AutoObjectParentSync = true;
            go.GetComponent<NetworkObject>().Spawn();
            EquipItem(new Item()
            {
                Id = 0,
                NetworkObjectId = go.GetComponent<NetworkObject>().NetworkObjectId,
            });
        }
        else if (IsClient) //equip item on load
        {
            NetworkObject go = GetNetworkObject(_equippedItem.Value.NetworkObjectId);

            go.GameObject().transform.SetParent(transform);
            go.GameObject().GetComponent<NetworkTransform>().enabled = false;
        }
    }
    void Update()
    {
        if (IsLocalPlayer && Input.GetKeyDown(KeyCode.E) && _equippedItem.Value.Equals(_emptyItem))
        {
            GameObject pickableObject = GetComponent<PlayerCamera>().GetFacingPickable();
            if (pickableObject != null)
            {
                EquipItem(new Item()
                {
                    Id = 0,
                    NetworkObjectId = pickableObject.GetComponent<NetworkObject>().NetworkObjectId,
                });
            }
        }
        if (IsLocalPlayer && Input.GetKeyDown(KeyCode.Q) && !_equippedItem.Value.Equals(_emptyItem))
        {
            UnequipItem();
        }
    }

    public void EquipItem(Item itemToEquip)
    {
        if (IsServer)
        {
            _equippedItem.Value = itemToEquip;

            NetworkObject equipped = GetNetworkObject(_equippedItem.Value.NetworkObjectId);
            equipped.TrySetParent(transform);

            foreach (Transform child in equipped.GameObject().transform)
            {
                child.gameObject.layer = 6;
            }

            GameObject equipGO = GetNetworkObject(_equippedItem.Value.NetworkObjectId).GameObject();

            equipGO.transform.transform.position = transform.position;
            equipGO.transform.transform.rotation = transform.rotation;
            equipGO.transform.transform.localPosition += new Vector3(0.5f, 0, 0);
            equipGO.GetComponent<Rigidbody>().isKinematic = true;
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
            NetworkObject unequipped = GetNetworkObject(_equippedItem.Value.NetworkObjectId);
            GetNetworkObject(_equippedItem.Value.NetworkObjectId).TryRemoveParent();
            _equippedItem.Value = _emptyItem;

            foreach (Transform child in unequipped.GameObject().transform)
            {
                child.gameObject.layer = 7;
            }

            unequipped.GameObject().GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            UnequipItemServerRpc();
        }
    }

    public override void OnNetworkSpawn()
    {
        _equippedItem.OnValueChanged += OnEquippedItemChange;
    }

    public override void OnNetworkDespawn()
    {
        _equippedItem.OnValueChanged -= OnEquippedItemChange;
    }

    private void OnEquippedItemChange(Item previousItem, Item newItem)
    {
        if (IsServer) return;

        if (!newItem.Equals(_emptyItem))
        {
            NetworkObject n = GetNetworkObject(newItem.NetworkObjectId);

            n.GameObject().transform.SetParent(transform);
            n.GameObject().GetComponent<NetworkTransform>().enabled = false;
            n.GetComponent<Rigidbody>().isKinematic = true;

            foreach (Transform child in n.GameObject().transform)
            {
                child.gameObject.layer = 6;
            }

        }
        else
        {
            NetworkObject prev = GetNetworkObject(previousItem.NetworkObjectId);

            prev.GameObject().transform.SetParent(null);
            prev.GameObject().GetComponent<NetworkTransform>().enabled = true;
            prev.GetComponent<Rigidbody>().isKinematic = false;

            foreach (Transform child in prev.GameObject().transform)
            {
                child.gameObject.layer = 0;
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
