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
            GetNetworkObject(_equippedItem.Value.NetworkObjectId).TrySetParent(transform);
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
            GetNetworkObject(_equippedItem.Value.NetworkObjectId).TryRemoveParent();
            _equippedItem.Value = _emptyItem;
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
        }
        else
        {
            NetworkObject prev = GetNetworkObject(previousItem.NetworkObjectId);

            prev.GameObject().GetComponent<NetworkTransform>().enabled = true;
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
