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
    void Start()
    {
        if (IsServer)
        {
            GameObject go = Instantiate(StickPrefab);
            go.GetComponent<NetworkObject>().AutoObjectParentSync = true;
            go.GetComponent<NetworkObject>().Spawn();
            _equippedItem.Value = new Item()
            {
                Id = 0,
                NetworkObjectId = go.GetComponent<NetworkObject>().NetworkObjectId,
            };
            Debug.Log(go.GetComponent<NetworkObject>().TrySetParent(GetComponent<NetworkObject>()));
        }
        else if (IsClient)
        {
            NetworkObject go = GetNetworkObject(_equippedItem.Value.NetworkObjectId);

            
            Debug.Log(_equippedItem.Value.NetworkObjectId);
            Debug.Log("sssssssssssss");
            Debug.Log(GetNetworkObject(_equippedItem.Value.NetworkObjectId).TrySetParent(transform));
            go.GameObject().transform.SetParent(transform);
            go.GameObject().GetComponent<NetworkTransform>().enabled = false;
        }
        //if (IsServer)
        //{
        //    _equippedItem.Value = new Item() { Id = 0, NetworkObjectId = Instantiate(StickPrefab).GetComponent<NetworkObject>().NetworkObjectId };
        //    NetworkObject i = GetNetworkObject(_equippedItem.Value.NetworkObjectId);
        //    i.Spawn();
        //    GetNetworkObject(_equippedItem.Value.NetworkObjectId).TrySetParent(GetComponent<NetworkObject>());
        //}
    }

    //public void EquipItem(Item itemToEquip)
    //{
    //    _equippedItem.Value = itemToEquip;
    //    GetNetworkObject(_equippedItem.Value.NetworkObjectId).TrySetParent(transform);
    //}


}
