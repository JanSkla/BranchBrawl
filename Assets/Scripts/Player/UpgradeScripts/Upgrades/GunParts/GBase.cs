using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GBase : GPart
{
    public GDestiny Destiny;
    public new void Shoot(ShootData shot)
    {
        Destiny.Part.Shoot(shot);
    }

    private NetworkList<ChildOnDestiny> _childOnDestinies = new();


    public override void OnNetworkSpawn()
    {
        if (_childOnDestinies != null && _childOnDestinies.Count > 0)
        {
            foreach (var child in _childOnDestinies)
            {
                NetworkSetParentOnDesinty(child);
            }
        }

        _childOnDestinies.OnListChanged += OnChODChange;
    }

    private void OnChODChange(NetworkListEvent<ChildOnDestiny> changeEvent)
    {
        if (changeEvent.Type == NetworkListEvent<ChildOnDestiny>.EventType.Add)
        {
            ChildOnDestiny newChOD = changeEvent.Value;

            NetworkSetParentOnDesinty(newChOD);
        }
    }

    private void NetworkSetParentOnDesinty(ChildOnDestiny ChOD)
    {
        NetworkObject childNwObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[ChOD.ChildNwId];
        GDestiny destiny = Destiny;

        if (childNwObject == null) Debug.LogError("There is no destiny with mentioned index");
        if (destiny == null) Debug.LogError("There is no destiny with mentioned index");

        childNwObject.AutoObjectParentSync = false;
        childNwObject.transform.SetParent(destiny.Position, false);
    }

    public void NetworkAddParentOnDestiny(int destinyIndex, ulong childNwId)
    {
        ChildOnDestiny chod = new()
        {
            DestinyIndex = destinyIndex,
            ChildNwId = childNwId
        };

        _childOnDestinies.Add(chod);
    }
}
