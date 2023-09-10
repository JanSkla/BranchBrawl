using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField]
    private static int _inventorySize = 3;

    private Item[] _inventory = new Item[_inventorySize];
    public NetworkVariable<Item> EquippedItem = new NetworkVariable<Item>();

    [SerializeField]
    private GameObject StickPrefab;

    public static Item _emptyItem = new Item();
    void Start()
    {
        if (IsServer)
        {
            //GameObject go = Instantiate(StickPrefab);
            //go.GetComponent<NetworkObject>().AutoObjectParentSync = true;
            //go.GetComponent<NetworkObject>().Spawn();
            //EquipItem(new Item()
            //{
            //    Id = 0,
            //    NetworkObjectId = go.GetComponent<NetworkObject>().NetworkObjectId,
            //});
        }
        else if (IsClient) //equip item on load
        {
            NetworkObject go = GetNetworkObject(EquippedItem.Value.NetworkObjectId);

            if (go != null)
            {
                go.GameObject().transform.SetParent(transform);
                go.GameObject().GetComponent<NetworkTransform>().enabled = false;
            }
        }
    }
    void Update()
    {
        if (IsLocalPlayer && Input.GetKeyDown(KeyCode.E) && EquippedItem.Value.Equals(_emptyItem))
        {
            GameObject pickableObject = transform.Find("Head").GetComponent<PlayerCamera>().GetFacingPickable();
            if (pickableObject != null)
            {
                if (pickableObject.CompareTag("Stick"))
                {
                    Vector3 stickPartLocalPos = pickableObject.transform.localPosition;
                    while (pickableObject.transform.parent != null)
                    {
                        pickableObject = pickableObject.transform.parent.gameObject;
                    }

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
                    while (pickableObject.transform.parent != null)
                    {
                        pickableObject = pickableObject.transform.parent.gameObject;
                    }

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

            NetworkObject equipped = GetNetworkObject(EquippedItem.Value.NetworkObjectId);

            equipped.TrySetParent(transform);

            int changeLayer = IsLocalPlayer ? 8 : 6;

            foreach (Transform child in equipped.GameObject().transform)
            {
                child.gameObject.layer = changeLayer;
                if (itemToEquip.PositionOffset != null)
                {
                    child.localPosition = child.localPosition - itemToEquip.PositionOffset;
                }
            }

            GameObject equipGO = GetNetworkObject(EquippedItem.Value.NetworkObjectId).GameObject();

            equipGO.transform.transform.position = transform.position;

            equipGO.transform.transform.rotation = transform.Find("Head").transform.rotation;
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
            NetworkObject unequipped = GetNetworkObject(EquippedItem.Value.NetworkObjectId);
            GetNetworkObject(EquippedItem.Value.NetworkObjectId).TryRemoveParent();
            EquippedItem.Value = _emptyItem;

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
            NetworkObject n = GetNetworkObject(newItem.NetworkObjectId);

            n.gameObject.transform.transform.position = transform.position;
            n.gameObject.transform.transform.rotation = transform.rotation;
            n.gameObject.transform.transform.localPosition += new Vector3(0.5f, 0, 0);
            n.gameObject.transform.SetParent(transform);
            n.gameObject.GetComponent<NetworkTransform>().enabled = false;
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
            NetworkObject prev = GetNetworkObject(previousItem.NetworkObjectId);

            prev.GameObject().transform.SetParent(null);
            prev.GameObject().GetComponent<NetworkTransform>().enabled = true;
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
