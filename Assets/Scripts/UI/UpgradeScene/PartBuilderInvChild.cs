using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PartBuilderInvChild : MonoBehaviour
{
    [SerializeField]
    private Texture2D handCursor; //TODO

    public int UpgradeId;

    private bool _isSelected;


    public PartBuilderInv PartBuilderInv;
    //obj ref
    public TextMeshProUGUI Count;
    public TextMeshProUGUI NamePlaceholder;

    public void SetGUpgradeData(GUpgradeData gud) //init called by PartBuilderInv on being Instanced
    {
        UpgradeId = gud.UpgradeId;
        Count.text = (gud.TotalCount - gud.UsedCount).ToString() + "x";
        NamePlaceholder.text = gud.PrefabResource;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;

        if (isSelected)
        {
            if (!PartBuilderInv.Selected.IsUnityNull())
            {
                PartBuilderInv.Selected.SetSelected(false);
            }

            PartBuilderInv.Selected = this;
            Cursor.SetCursor(handCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            PartBuilderInv.Selected = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnClick()
    {
        SetSelected(!_isSelected);
    }
}