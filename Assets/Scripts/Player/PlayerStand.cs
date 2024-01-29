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
            _readyText.text = "READY";
            _readyText.color = new Color(0.0235f, 0.7451f, 0.0941f);
        }
        else
        {
            _readyText.text = "UNREADY";
            _readyText.color = Color.red;
        }
    }
    public void SetNameVisibility(bool visibility)
    {
        _nickText.enabled = visibility;
        _readyText.enabled = visibility;
    }
}
