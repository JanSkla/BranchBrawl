using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GEmptyEnhancer : GEnhancer
{
    public GEmptyEnhancer()
    {
        Destiny = new GDestiny[1];
    }
    public new void Shoot(ShootData shot, Player owner)
    {
        Destiny[0].Part.Shoot(shot, owner);
    }
}
