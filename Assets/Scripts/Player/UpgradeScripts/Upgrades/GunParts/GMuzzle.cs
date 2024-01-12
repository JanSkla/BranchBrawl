using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMuzzle : GPart
{
    public new void Shoot(ShootData shot)
    {
        Debug.Log("Shoots successfully");
    }

    public void ReplaceMuzzle(UpgradeWithPart guPrefab, bool isNetwork = false)
    {
        Debug.Log("ReplaceMuzzle");
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
            gu.NetworkObject.TrySetParent(parentGDestRef.Position, false); //CHECK WHY FALSE NOT SETTING PARENT
        }
        else
        {
            gu.NetworkObject.AutoObjectParentSync = false;
            gu.transform.SetParent(parentGDestRef.Position, false);
        }
        parentGDestRef.Part = gu;

        int guDLength = gu.Destiny.Length;

        for (int i = 0; i < guDLength; i++) //spawn muzzle for new endings
        {
            if (isNetwork)
                PlayerGunManager.NetworkGBDMuzzleInstantiateOnDestiny(gu.Destiny[i]);
            else
                PlayerGunManager.GBDMuzzleInstantiateOnDestiny(gu.Destiny[i]);
        }

        DestroyPartRecursive();
    }
}
