using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Action<bool, Player> shootInput; //true == first press in series
    [SerializeField]
    private Player player;

    void Update()
    {
        if (!player.AreControlsDisabled && player.IsLocalPlayer && player.IsAlive && shootInput != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                shootInput.Invoke(true, player);
            }
            else if (Input.GetMouseButton(0))
            {
                shootInput.Invoke(false, player);
            }
        }
    }
}
