using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GEmptyEnhancer : GEnhancer
{
    public GEmptyEnhancer()
    {
        Destiny = new GPart[1];
    }
    public new void Shoot(ShootData shot)
    {
        Destiny[0].Shoot(shot);
    }
}
