using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GBase : GPart
{
    private static ShootData _shotData = new() { Amount = 40 };

    public GDestiny Destiny;
    public override void Shoot(bool firstShot, ShootData shootData)
    {

    }
    public void Shoot(bool firstShot)
    {
        Debug.Log(name);
        Destiny.Part.Shoot(firstShot, _shotData);
    }

    private NetworkList<ulong> _childsOnDesitny = new();


    public override void OnNetworkSpawn()
    {
        if (_childsOnDesitny != null && _childsOnDesitny.Count > 0)
        {
            foreach (var child in _childsOnDesitny)
            {
                NetworkSetParentOnDesinty(child);
            }
        }

        _childsOnDesitny.OnListChanged += OnChODChange;
    }

    private void OnChODChange(NetworkListEvent<ulong> changeEvent)
    {
        if (changeEvent.Type == NetworkListEvent<ulong>.EventType.Add)
        {
            ulong newChildNwIdD = changeEvent.Value;

            NetworkSetParentOnDesinty(newChildNwIdD);
        }
    }

    private void NetworkSetParentOnDesinty(ulong childNwId)
    {
        NetworkObject childNwObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[childNwId];

        if (childNwObject == null) Debug.LogError("There is no destiny with mentioned index");

        childNwObject.AutoObjectParentSync = false;
        childNwObject.transform.SetParent(Destiny.PositionPoint.transform, false);
    }

    public void NetworkAddParentOnDestiny(ulong childNwId)
    {
        _childsOnDesitny.Add(childNwId);
    }
}
