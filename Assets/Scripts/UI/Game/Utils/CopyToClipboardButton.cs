using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CopyToClipboardButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textToCopy;
    
    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = textToCopy.text;
        Debug.Log("copied: " + textToCopy.text);
    }
}
