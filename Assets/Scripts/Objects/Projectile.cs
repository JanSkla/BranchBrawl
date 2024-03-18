using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Projectile : NetworkBehaviour
{
    public int DamageAmount;
    public Player Owner;
    [SerializeField]
    private float _forceMultiplier;
    [SerializeField]
    protected Rigidbody _rb;
    public void ApplyForce(Vector3 facing)
    {
        _rb.AddRelativeForce(facing * _forceMultiplier);
    }
    protected void DestroySelf()
    {
        GetComponent<NetworkObject>().Despawn();
        Utils.DestroyWithChildren(gameObject);
    }
}
