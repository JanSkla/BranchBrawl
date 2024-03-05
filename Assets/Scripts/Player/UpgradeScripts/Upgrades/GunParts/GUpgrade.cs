using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
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
        destiny.Part = childNwObject.GetComponent<GPart>();
        GUpgrade gUpgrade = destiny.Part.GetComponent<GUpgrade>();

        if (gUpgrade.IsUnityNull()) return;

        var parent = transform.parent;
        Debug.Log(parent);
        var gPoint = parent.GetComponent<GPoint>();

        GDestiny parentDest;
        if (gPoint.Parent.GetComponent<GBase>())
        {
            parentDest = gPoint.Parent.GetComponent<GBase>().Destiny;
        }
        else if (gPoint.Parent.GetComponent<GUpgrade>())
        {
            parentDest = gPoint.Parent.GetComponent<GUpgrade>().Destiny[gPoint.DestinyIndex];
        }
        else return;


        //Debug.Log(parentDest);
        //int[] previousUpgradeIds = new int[parentDest.PreviousUpgradeIds.Length + 1];
        //for (int i = 0; i < parentDest.PreviousUpgradeIds.Length; i++)
        //{
        //    previousUpgradeIds[i] = parentDest.PreviousUpgradeIds[i];
        //}
        //previousUpgradeIds[parentDest.PreviousUpgradeIds.Length] = gUpgrade.UpgradeId;
        //foreach (GDestiny dest in gUpgrade.Destiny)
        //{
        //    dest.PreviousUpgradeIds = previousUpgradeIds;
        //}
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

        GameObject parentparentGO = gp.Parent.gameObject;

        GDestiny parentGDestRef;

        if (parentparentGO.GetComponent<GUpgrade>())
        {
            int di = gp.DestinyIndex;

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

            int[] previousUpgradeIds = new int[parentGDestRef.PreviousUpgradeIds.Length + 1];
            for (int i = 0; i < parentGDestRef.PreviousUpgradeIds.Length; i++)
            {
                previousUpgradeIds[i] = parentGDestRef.PreviousUpgradeIds[i];
            }
            previousUpgradeIds[parentGDestRef.PreviousUpgradeIds.Length] = gu.UpgradeId;
            foreach (GDestiny dest in gu.Destiny)
            {
                dest.PreviousUpgradeIds = previousUpgradeIds;
            }

        }
        parentGDestRef.Part = gu;

        int guDLength = gu.Destiny.Length;

        for (int i = 0; i < Destiny.Length; i++)
        {
            if (i < guDLength) //keep existing
            {
                gu.Destiny[i].Part = Destiny[i].Part;
                Destiny[i].Part = null;

                if (isNetwork)
                {
                    gu.Destiny[i].Part.NetworkObject.TrySetParent(gu.Destiny[i].PositionPoint.transform, false);
                }
                else
                {
                    gu.Destiny[i].Part.transform.SetParent(gu.Destiny[i].PositionPoint.transform, false);
                }
            }
            else //destroy overflowing
            {
                Destiny[i].Part.DestroyPartRecursive();
            }
        }

        foreach (GDestiny dest in gu.Destiny)
        {
            RefreshPrevUIdDataForAllChilds(dest);
        }

        for (int i = Destiny.Length; i < guDLength; i++) //spawn muzzle for new endings
        {
            if (isNetwork)
                MuzzleManager.NetworkMuzzleInstantiateOnDestiny(gu.Destiny[i]);
            else
                MuzzleManager.MuzzleInstantiateOnDestiny(gu.Destiny[i]);
        }

        DestroyPartRecursive();
    }
    private static void RefreshPrevUIdDataForAllChilds(GDestiny originDestiny)
    {
        if (!originDestiny.Part) return;
        var part = originDestiny.Part.GetComponent<GUpgrade>();

        if (part)
        {
            foreach (var destiny in part.Destiny)
            {
                destiny.PreviousUpgradeIds = new int[originDestiny.PreviousUpgradeIds.Length + 1];
                for (int i = 0; i < originDestiny.PreviousUpgradeIds.Length; i++)
                {
                    destiny.PreviousUpgradeIds[i] = originDestiny.PreviousUpgradeIds[i];
                }
                destiny.PreviousUpgradeIds[originDestiny.PreviousUpgradeIds.Length] = part.UpgradeId;
                RefreshPrevUIdDataForAllChilds(destiny);
            }
        }
        else
        {
            Utils.DestroyWithChildren(originDestiny.Part.gameObject);
            MuzzleManager.MuzzleInstantiateOnDestiny(originDestiny);
        }
    }
}
