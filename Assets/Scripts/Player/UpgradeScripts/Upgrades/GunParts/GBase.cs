using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBase : MonoBehaviour
{
    public GDestiny Destiny;
    public void Shoot(ShootData shot)
    {
        Destiny.Part.Shoot(shot);
    }
}
