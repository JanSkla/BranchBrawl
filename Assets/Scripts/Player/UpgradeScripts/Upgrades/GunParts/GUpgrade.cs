using System.Collections;
using System.Collections.Generic;
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
            gu.NetworkObject.TrySetParent(parentGDestRef.Position, false);
        }
        else
        {
            gu.NetworkObject.AutoObjectParentSync = false;
            gu.transform.SetParent(parentGDestRef.Position, false);
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
                    Destiny[i].Part.NetworkObject.TrySetParent(gu.Destiny[i].Position, false);
                }
                else
                {
                    Destiny[i].Part.transform.SetParent(gu.Destiny[i].Position, false);
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
