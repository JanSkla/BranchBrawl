using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GSplitter : GUpgrade
{
    public new void Shoot(ShootData shot)
    {
        foreach (var dest in Destiny)
        {
            dest.Shoot(shot);
        }
    }
}
