using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMuzzle : GPart
{
    public new void Shoot(ShootData shot)
    {
        Debug.Log("Shoots successfully");
    }

    public void ReplaceMuzzle(UpgradeWithPart guPrefab)
    {
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

        gu.transform.SetParent(parentGDestRef.Position, false);
        parentGDestRef.Part = gu;

        int guDLength = gu.Destiny.Length;

        for (int i = 0; i < guDLength; i++) //spawn muzzle for new endings
        {
            GameObject gMuzzleprefab = Resources.Load("Prefabs/GunParts/GMuzzle") as GameObject;
            GMuzzle gMuzzle = Instantiate(gMuzzleprefab).GetComponent<GMuzzle>();

            Debug.Log(gMuzzle);

            gMuzzle.transform.SetParent(gu.Destiny[i].Position, false);

            gu.Destiny[i].Part = gMuzzle;
        }
    }
}
