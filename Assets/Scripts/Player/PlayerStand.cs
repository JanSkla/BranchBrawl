using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStand : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _nickText;

    [SerializeField]
    private TextMeshPro _readyText;

    public void SetNickname(string name)
    {
        _nickText.text = name;
    }
    public void SetReady(bool isReady)
    {
        if (isReady)
        {
            _nickText.text = "Ready";
            _nickText.color = Color.green;
        }
        else
        {
            _nickText.text = "Unready";
            _nickText.color = Color.red;
        }
    }
    public void SetNameVisibility(bool visibility)
    {
        _nickText.enabled = visibility;
        _readyText.enabled = visibility;
    }
}
