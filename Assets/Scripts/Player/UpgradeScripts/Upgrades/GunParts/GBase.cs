using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBase : MonoBehaviour
{
    public GPart destiny;
    public void Shoot(ShootData shot)
    {
        destiny.Shoot(shot);
    }
}
