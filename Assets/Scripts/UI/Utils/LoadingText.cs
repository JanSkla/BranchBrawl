using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    private void Update()
    {
        switch (Time.frameCount/60 % 4)
        {
            case 0:
                _text.text = "Loading";
                break;
            case 1:
                _text.text = "Loading.";
                break;
            case 2:
                _text.text = "Loading..";
                break;
            case 3:
                _text.text = "Loading...";
                break;
        }
    }
}
