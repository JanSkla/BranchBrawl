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
        Invoke(nameof(DelayedStart), 0.5f);
        _healthDisplay.gameObject.SetActive(false);
    }
    private void DelayedStart()
    {
        _healthDisplay.gameObject.SetActive(true);
        var health = _inGameUI.GetComponent<InGameUI>().CurrentPlayer.GetComponent<PlayerHealth>().Health;
        health.OnValueChanged += UpdateDisplay;
        UpdateDisplay(0, health.Value);
    }

    private void UpdateDisplay(int _prevHealth, int newHealth)
    {
        _healthDisplay.ChangeAmount(_prevHealth, newHealth);
    }
}
