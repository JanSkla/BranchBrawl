using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GEnhancer : GUpgrade
{
    public override void Shoot(ShootData shot, Player owner)
    {
        Destiny[0].Part.Shoot(shot, owner);
    }
    public int EnhanceScript(int amount)
    {
        return amount;
    }
}
