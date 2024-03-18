using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthDisplayText : MonoBehaviour
{
    [SerializeField]
    private HealthDisplay _healthDisplay;

    [SerializeField]
    private GameObject _inGameUI;

    void Start()
    {
        _healthDisplay.gameObject.SetActive(false);
    }
    public void ConnectHealthToPlayer(Player player)
    {
        _healthDisplay.gameObject.SetActive(true);
        var health = player.GetComponent<PlayerHealth>().Health;
        health.OnValueChanged += UpdateDisplay;
        UpdateDisplay(0, health.Value);
    }

    private void UpdateDisplay(int _prevHealth, int newHealth)
    {
        _healthDisplay.ChangeAmount(_prevHealth, newHealth);
    }
}
