using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyBtn : MonoBehaviour
{
    private Button _button;
    private bool _isPressed;
    private void Start()
    {
        _button = GetComponent<Button>();
    }
    public void OnClick()
    {
        _isPressed = !_isPressed;
        ColorBlock colors = _button.colors;
        colors.selectedColor = _isPressed ? new Color(0.06666667f, 0.6235294f, 0f): new Color(0.5566038f, 0f, 0.02638939f);
        _button.colors = colors;
    }
}
