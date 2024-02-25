using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public abstract class GMuzzle : GPart
{
    public void ReplaceMuzzle(UpgradeWithPart guPrefab, bool isNetwork = false)
    {
        Debug.Log("ReplaceMuzzle");
        GUpgrade gu = guPrefab.InstantiatePrefab();

        GPoint gp = transform.parent.GetComponent<GPoint>();

        GameObject parentparentGO = gp.Parent;

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
            gu.NetworkObject.TrySetParent(parentGDestRef.PositionPoint.transform, false); //TODO
        }
        else
        {
            gu.NetworkObject.AutoObjectParentSync = false;
            gu.transform.SetParent(parentGDestRef.PositionPoint.transform, false);
        }
        parentGDestRef.Part = gu;

        int guDLength = gu.Destiny.Length;

        for (int i = 0; i < guDLength; i++) //spawn muzzle for new endings
        {
            if (isNetwork)
                MuzzleManager.NetworkMuzzleInstantiateOnDestiny(gu.Destiny[i]);
            else
                MuzzleManager.MuzzleInstantiateOnDestiny(gu.Destiny[i]);
        }

        DestroyPartRecursive();
    }
}
