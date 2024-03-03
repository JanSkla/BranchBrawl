using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField]
    private float _forceMultiplier;
    public int DamageAmount;
    //public NetworkVariable<ulong> OwnerNwId;
    public Player Owner;
    [SerializeField]
    private Rigidbody _rb;

    private bool _collided = false;
    private bool _collidedWPlayer = false;


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

        if (_collidedWPlayer) return;
        if (!NetworkManager.IsServer) return;

        if (!_collided)
        {
            _collided = true;
            Invoke(nameof(DestroySelf), 3);
        }

        var targetPlayer = collision.gameObject.GetComponent<Player>();
        if (targetPlayer.IsUnityNull()) return;

        if (Owner == targetPlayer) return;

        _collidedWPlayer = true;
        targetPlayer.GetComponent<PlayerHealth>().Damage(DamageAmount);
    }

    private void DestroySelf()
    {
        Utils.DestroyWithChildren(gameObject);
    }
}
