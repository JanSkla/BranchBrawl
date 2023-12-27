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
}
