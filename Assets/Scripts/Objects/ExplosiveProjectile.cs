using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    //public NetworkVariable<ulong> OwnerNwId;
    [SerializeField]
    private float _radius;
    [SerializeField]
    private GameObject _explosionVisualisation;

    private bool _collided = false;


    private void OnCollisionEnter(Collision collision)
    {
        if (_collided) return;

        _collided = true;

        if (!NetworkManager.IsServer) return;

        Collider[] hits = Physics.OverlapSphere(
          transform.position,
          _radius
          //LayerMask.NameToLayer("Player")
          );

        _rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        _rb.isKinematic = true;
        Debug.Log(LayerMask.NameToLayer("Player"));
        _explosionVisualisation.SetActive(true);

        foreach (var hit in hits)
        {
            Debug.Log(hit.name);

            var player = hit.GetComponent<Player>();
            if (player)
            {
                var health = player.GetComponent<PlayerHealth>();
                health.Damage(DamageAmount);
            }
        }
        Invoke(nameof(DestroySelf), 2);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position,
          _radius);
    }
}
