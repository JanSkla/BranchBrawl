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

    public void Damage(int amount)
    {
        if (!IsALive()) return;
        Health.Value -= amount;

        var localPlayer = GetComponent<LocalPlayer>();
        if (localPlayer.enabled)
        {
            localPlayer.InGameUI.Game.GetComponent<GameUI>().DamageHueAnimator.SetTrigger("WasDamaged");
        }
    }

    private void Heal(int amount)
    {
        int currentHealth = Health.Value;
        if (currentHealth + amount >= maxHealth)
        {
            Health.Value = maxHealth;
        }
        else
        {
            Health.Value += amount;
        }
    }

    private void Die()
    {
        GetComponent<Player>().RigAnimator.SetBool("IsDead", true);
        GetComponent<Player>().IsAlive = false;
    }

    private bool IsALive() => Health.Value > 0;
}
