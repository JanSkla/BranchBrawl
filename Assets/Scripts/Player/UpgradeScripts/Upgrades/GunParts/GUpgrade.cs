using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GUpgrade : GPart
{
    [SerializeField]
    private GPart[] _destiny;
    public GPart[] Destiny
    {
        get { return _destiny; }
        set { _destiny = value; }
    }
}
