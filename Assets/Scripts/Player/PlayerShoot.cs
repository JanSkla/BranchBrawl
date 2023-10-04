using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Action shootInput;
    [SerializeField]
    private Player player;

    void Update()
    {
        if (player.IsLocalPlayer && shootInput != null && Input.GetMouseButton(0))
        {
            Debug.Log("tries to invoke shoot");
            shootInput.Invoke();
        }
    }
}
