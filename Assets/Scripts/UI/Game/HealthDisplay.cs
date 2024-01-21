using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _healthDisplay;
    [SerializeField]
    private Image _underAmountImage;
    [SerializeField]
    private Image _amountImage;

    [SerializeField]
    private Color _lowColor;

    [SerializeField]
    private Color _fullColor;

    public void ChangeAmount(float prevVal, float value)
    {
        StartCoroutine(Tools.SmoothLerpMoveTo(prevVal, value, 1, RenderAmount));
    }

    private void RenderAmount(float value)
    {
        float amount = value > 100 ? 1 : value / 100;

        _amountImage.fillAmount = amount;

        _amountImage.color = Color.Lerp(_lowColor, _fullColor, amount);

        _healthDisplay.text = Mathf.Ceil(value).ToString();
    }
    private void RenderAmount(float originVal, float goalAmount, float progress)
    {
        float value = Mathf.Lerp(originVal, goalAmount, progress);

        RenderAmount(value);

        if (progress != 1) return;

        _underAmountImage.fillAmount = value > 100 ? 1 : value / 100;
    }
}
