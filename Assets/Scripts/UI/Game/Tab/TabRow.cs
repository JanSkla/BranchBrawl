using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabRow : MonoBehaviour
{
    public TextMeshProUGUI PlayerName;
    [SerializeField]
    private Image[] _crowns;

    private int _crownCount;
    public int CrownCount { get { return _crownCount; }
        set
        {
            _crownCount = value;
            for (int i = 0; i < _crowns.Length; i++)
            {
                _crowns[i].color = i < value ? Color.white : new(0.21f, 0.21f, 0.21f);
            }
        }
    }
}
