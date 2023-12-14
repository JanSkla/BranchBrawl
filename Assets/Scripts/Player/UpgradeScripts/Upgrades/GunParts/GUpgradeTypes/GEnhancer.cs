using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GEnhancer : GUpgrade, IHasDestiny, IHasSource
{
    private IHasSource[] _destiny;
    public IHasSource[] Destiny
    {
        get { return _destiny; }
        set { _destiny = value; }
    }

    private IHasDestiny _source;
    public IHasDestiny Source
    {
        get { return _source; }
        set { _source = value; }
    }
    public void Shoot(int amount)
    {
        Destiny[0].Shoot(amount);
    }
}
