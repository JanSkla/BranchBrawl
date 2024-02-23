using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField]
    private Vector3 _force;
    private int _damageAmount;
    private Player _owner;
    [SerializeField]
    private Rigidbody _rb;


    void Start()
    {
        ApplyForce();
    }

    private void ApplyForce()
    {
        //if (!NetworkManager.IsServer) return;

        _rb.AddForce(_force);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!NetworkManager.IsServer) return;

        var targetPlayer = collision.gameObject.GetComponent<Player>();
        if (targetPlayer.IsUnityNull()) return;

        targetPlayer.GetComponent<PlayerHealth>().Damage(_damageAmount);
    }
}
