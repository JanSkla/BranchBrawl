using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GSplitter : GUpgrade
{
    public override void Shoot(bool firstShot, ShootData shot)
    {
        foreach (var dest in Destiny)
        {
            dest.Part.Shoot(firstShot, shot);
        }
    }
}
