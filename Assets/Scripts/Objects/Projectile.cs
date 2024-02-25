using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField]
    private float _forceMultiplier;
    private int _damageAmount;
    private Player _owner;
    [SerializeField]
    private Rigidbody _rb;


    void Start()
    {
        Debug.Log("projectile here");
        //ApplyForce();
    }

    public void ApplyForce(Vector3 facing)
    {
        //if (!NetworkManager.IsServer) return;

        _rb.AddRelativeForce(facing*_forceMultiplier);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!NetworkManager.IsServer) return;

        var targetPlayer = collision.gameObject.GetComponent<Player>();
        if (targetPlayer.IsUnityNull()) return;

        targetPlayer.GetComponent<PlayerHealth>().Damage(_damageAmount);
    }
}
