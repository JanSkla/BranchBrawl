using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Action<bool> shootInput; //true == first press in series
    [SerializeField]
    private Player player;

    void Update()
    {
        Debug.Log(1);
        if (player.IsLocalPlayer && player.IsAlive && shootInput != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                shootInput.Invoke(true);
            }
            else if (Input.GetMouseButton(0))
            {
                shootInput.Invoke(false);
            }
        }
    }
}
