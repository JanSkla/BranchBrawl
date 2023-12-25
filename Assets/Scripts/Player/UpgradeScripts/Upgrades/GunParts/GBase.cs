using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBase : GPart
{
    public GDestiny Destiny;
    public new void Shoot(ShootData shot)
    {
        Destiny.Part.Shoot(shot);
    }
}
