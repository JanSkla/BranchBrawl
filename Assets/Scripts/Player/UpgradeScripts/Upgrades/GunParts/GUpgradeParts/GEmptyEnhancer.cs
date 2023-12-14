using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GEmptyEnhancer : GEnhancer
{
    public GEmptyEnhancer()
    {
        Destiny = new IHasSource[1];
    }
    public new void Shoot(int amount)
    {
        Destiny[0].Shoot(amount);
    }
}
