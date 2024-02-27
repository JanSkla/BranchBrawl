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
        Debug.Log("Collided" + NetworkManager.IsServer);
        if (!NetworkManager.IsServer) return;

        var targetPlayer = collision.gameObject.GetComponent<Player>();
        Debug.Log("Collided" + 1 + targetPlayer);
        if (targetPlayer.IsUnityNull()) return;

        Debug.Log("Collided" + 2);
        if (Owner == targetPlayer) return;
        Debug.Log("Collided" + 3);

        targetPlayer.GetComponent<PlayerHealth>().Damage(DamageAmount);
    }
}
