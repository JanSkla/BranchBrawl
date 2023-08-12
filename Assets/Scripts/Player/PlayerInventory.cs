using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
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
            go.GetComponent<NetworkObject>().Spawn();

            if (!go.GetComponent<NetworkObject>().AutoObjectParentSync)
            {
                Debug.Log("1");
                go.GetComponent<NetworkObject>().AutoObjectParentSync = true;
                Debug.Log(go.GetComponent<NetworkObject>().AutoObjectParentSync);
            }

            if (go.GetComponent<NetworkObject>().NetworkManager == null || !go.GetComponent<NetworkObject>().NetworkManager.IsListening)
            {
                Debug.Log("2");
            }

            if (!go.GetComponent<NetworkObject>().NetworkManager.IsServer)
            {
                Debug.Log("3");
            }

            if (!go.GetComponent<NetworkObject>().IsSpawned)
            {
                Debug.Log("4");
            }
            if (GetComponent<NetworkObject>() != null && !GetComponent<NetworkObject>().IsSpawned)
            {
                Debug.Log("5");
            }
            Debug.Log(go.GetComponent<NetworkObject>().TrySetParent(GetComponent<NetworkObject>()));
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
