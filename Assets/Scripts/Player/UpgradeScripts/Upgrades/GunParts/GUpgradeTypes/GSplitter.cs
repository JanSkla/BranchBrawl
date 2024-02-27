using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GSplitter : GUpgrade
{
    public override void Shoot( ShootData shot, Player owner)
    {
        foreach (var dest in Destiny)
        {
            dest.Part.Shoot(shot, owner);
        }
    }
}
