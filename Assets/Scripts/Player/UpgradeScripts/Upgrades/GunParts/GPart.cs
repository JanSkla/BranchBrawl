using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GPart : MonoBehaviour
{
    public void Shoot(ShootData shot) { }
}
[Serializable]
public class GDestiny
{
    public Transform position;
    public GPart Part;
}
