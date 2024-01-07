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

    public void ReplacePart(GUpgrade guPrefab) //TODO needs check
    {
        GUpgrade gu = Instantiate(guPrefab);

        GameObject parentparentGO = gu.transform.parent.parent.gameObject;

        GDestiny parentGDestRef;

        if (parentparentGO.GetComponent<GUpgrade>())
        {
            int di = gu.transform.parent.GetComponent<GPoint>().destinyIndex;

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

        gu.transform.SetParent(parentGDestRef.Position);
        parentGDestRef.Part = gu;

        int guDLength = gu.Destiny.Length;

        for (int i = 0; i < Destiny.Length; i++)
        {
            if (i < guDLength)
            {
                gu.Destiny[i].Part = Destiny[i].Part;
                Destiny[i].Part.transform.SetParent(gu.Destiny[i].Position);
            }
            else
            {
                Destiny[i].Part.DestroyPartRecursive();
            }
        }
    }
}
