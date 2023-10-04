using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunData gunData;
    private float timeSinceLastShot = 0f;

    void Start()
    {
    }
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    public void Shoot()
    {
        Debug.Log("tries to shoot");
        if (!CanShoot() || gunData.currentAmmo <= 0) return;

        Debug.Log(gunData.name + " Shoots");
        Debug.DrawRay(transform.position, transform.forward * 100, Color.red, 1);
        timeSinceLastShot = 0f;
    }

    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / gunData.firerate;
}
