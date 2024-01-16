using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GBase : GPart
{
    [SerializeField]
    private GunData gunData;

    public GDestiny Destiny;

    private static ShootData _shotData = new() { Amount = 40, Type = ShootType.Bullet };


    private float _timeSinceLastShot = 0f;

    void Update()
    {
        _timeSinceLastShot += Time.deltaTime;
    }

    public void Shoot(bool firstShot)
    {
        if (!gunData.isAuto && !firstShot) return;
        if (!CanShoot() || gunData.currentAmmo <= 0) return;

        Destiny.Part.Shoot(_shotData);

        gunData.currentAmmo--;
        _timeSinceLastShot = 0f;
    }
    private bool CanShoot() => !gunData.reloading && _timeSinceLastShot > 1f / gunData.firerate;

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
        Destiny.Part = childNwObject.GetComponent<GPart>();
    }

    public void NetworkAddParentOnDestiny(ulong childNwId)
    {
        _childsOnDesitny.Add(childNwId);
    }
    public override void Shoot(ShootData shootData) { }
}
