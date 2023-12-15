using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBase : MonoBehaviour
{
    public GDestiny destiny;
    public void Shoot(ShootData shot)
    {
        destiny.Part.Shoot(shot);
    }
}
