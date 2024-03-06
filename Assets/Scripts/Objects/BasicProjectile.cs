using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BasicProjectile : Projectile
{
    private bool _collided = false;
    private bool _collidedWPlayer = false;


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
}
