using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField]
    private static int maxHealth = 100;

    public NetworkVariable<int> Health = new NetworkVariable<int>(maxHealth);

    public override void OnNetworkSpawn()
    {
        Health.OnValueChanged += OnServerHealthChange;
    }
    public override void OnNetworkDespawn()
    {
        Health.OnValueChanged -= OnServerHealthChange;
    }

    private void OnServerHealthChange(int _prevHealth, int newHealth)
    {
        if (newHealth <= 0)
        {
            Die();
        }
    }

    private void Damage(int amount)
    {
        Health.Value -= amount;
        Die();
    }

    private void Heal(int amount)
    {
        Health.Value += amount;
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died");
    }
}
