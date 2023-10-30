using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapons")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    public new string name;

    [Header("Shooting")]
    public int damage;
    public float firerate;
    public bool isAuto;

    [Header("Reloading")]
    public int currentAmmo;
    public int magSize;
    public float reloadTime;
    [HideInInspector]
    public bool reloading = false;

    [Header("Other")]
    public float movementSpeed;
    public float accuracy;
    public float recoil;
}
