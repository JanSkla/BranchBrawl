using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthDisplayText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _healthDisplay;

    [SerializeField]
    private GameObject _inGameUI;

    void Start()
    {
        _healthDisplay.enabled = false;
        Invoke(nameof(DelayedStart), 0.5f);
    }
    private void DelayedStart()
    {
        var health = _inGameUI.GetComponent<InGameUI>().CurrentPlayer.GetComponent<PlayerHealth>().Health;
        health.OnValueChanged += UpdateDisplay;
        UpdateDisplay(0, health.Value);
        _healthDisplay.enabled = true;
    }

    private void UpdateDisplay(int _prevHealth, int newHealth)
    {
        _healthDisplay.text = newHealth.ToString();
    }
}
