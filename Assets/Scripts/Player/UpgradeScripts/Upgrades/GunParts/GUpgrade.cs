using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class GUpgrade : GPart
{
    public int UpgradeId;
    [SerializeField]
    private GDestiny[] _destiny;
    public GDestiny[] Destiny
    {
        get { return _destiny; }
        set { _destiny = value; }
    }

    private NetworkList<ChildOnDestiny> _childOnDestinies = new();


    public override void OnNetworkSpawn()
    {
        if(_childOnDestinies != null && _childOnDestinies.Count > 0)
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
        if(changeEvent.Type == NetworkListEvent<ChildOnDestiny>.EventType.Add)
        {
            ChildOnDestiny newChOD = changeEvent.Value;

            NetworkSetParentOnDesinty(newChOD);
        }
    }

    private void NetworkSetParentOnDesinty(ChildOnDestiny ChOD)
    {
        NetworkObject childNwObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[ChOD.ChildNwId];
        GDestiny destiny = _destiny[ChOD.DestinyIndex];

        if (childNwObject == null) Debug.LogError("There is no destiny with mentioned index");
        if (destiny == null) Debug.LogError("There is no destiny with mentioned index");

        childNwObject.AutoObjectParentSync = false;
        childNwObject.transform.SetParent(destiny.PositionPoint.transform, false);
    }

    public void NetworkAddParentOnDestiny(int destinyIndex, ulong childNwId)
    {
        ChildOnDestiny chod = new() { DestinyIndex = destinyIndex,
            ChildNwId = childNwId };

        _childOnDestinies.Add(chod);
    }

    public void ReplacePart(UpgradeWithPart guPrefab, bool isNetwork = false)
    {
        Debug.Log("ReplacePart");
        GUpgrade gu = guPrefab.InstantiatePrefab();

        GPoint gp = transform.parent.GetComponent<GPoint>();

        GameObject parentparentGO = gp.parent;

        GDestiny parentGDestRef;

        if (parentparentGO.GetComponent<GUpgrade>())
        {
            int di = gp.destinyIndex;

            parentGDestRef = parentparentGO.GetComponent<GUpgrade>().Destiny[di];
        }
        else if (parentparentGO.GetComponent<GBase>())
        {
            parentGDestRef = parentparentGO.GetComponent<GBase>().Destiny;
        }
        else
        {
            Debug.Log("There is no GUpgrade nor GBase");
            return;
        }

        if (isNetwork)
        {
            gu.NetworkObject.Spawn();
            gu.NetworkObject.TrySetParent(parentGDestRef.PositionPoint.transform, false);
        }
        else
        {
            gu.NetworkObject.AutoObjectParentSync = false;
            gu.transform.SetParent(parentGDestRef.PositionPoint.transform, false);
        }
        parentGDestRef.Part = gu;

        int guDLength = gu.Destiny.Length;

        Debug.Log("gud" + guDLength);
        Debug.Log("d" + Destiny.Length);

        for (int i = 0; i < Destiny.Length; i++)
        {
            if (i < guDLength) //keep exissting
            {
                gu.Destiny[i].Part = Destiny[i].Part;

                if (isNetwork)
                {
                    Destiny[i].Part.NetworkObject.TrySetParent(gu.Destiny[i].PositionPoint.transform, false);
                }
                else
                {
                    Destiny[i].Part.transform.SetParent(gu.Destiny[i].PositionPoint.transform, false);
                }
            }
            else //destroy overflowing
            {
                Destiny[i].Part.DestroyPartRecursive();
            }
        }
        for (int i = Destiny.Length; i < guDLength; i++) //spawn muzzle for new endings
        {
            if (isNetwork)
                PlayerGunManager.NetworkMuzzleInstantiateOnDestiny(gu.Destiny[i]);
            else
                PlayerGunManager.MuzzleInstantiateOnDestiny(gu.Destiny[i]);
        }

        DestroyPartRecursive();
    }
}
