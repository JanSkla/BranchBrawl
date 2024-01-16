using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GEnhancer : GUpgrade
{
    public override void Shoot(ShootData shot)
    {
        Destiny[0].Part.Shoot(shot);
    }
    public int EnhanceScript(int amount)
    {
        return amount;
    }
}
