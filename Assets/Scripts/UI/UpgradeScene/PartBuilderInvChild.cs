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
    public int UpgradeId;

    private bool _isSelected;


    public PartBuilderInv PartBuilderInv;
    //obj ref
    public TextMeshProUGUI Count;
    public Image Icon;

    public void SetGUpgradeData(GUpgradeData gud) //init called by PartBuilderInv on being Instanced
    {
        UpgradeId = gud.UpgradeId;
        Count.text = (gud.TotalCount - gud.UsedCount).ToString() + "x";
        Icon.sprite = UpgradeManager.GetUpgradeById(gud.UpgradeId).GetIconSprite();

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
            var gp = GameObject.Find("GunPlaceholder").GetComponent<GunPlaceholder>();


            if (gp.IsDelete)
                gp.ToggleDelete();

            PartBuilderInv.Selected = this;
            CursorHandler.Hand();
        }
        else
        {
            PartBuilderInv.Selected = null;
            CursorHandler.Default();
        }
    }

    private void OnClick()
    {
        SetSelected(!_isSelected);
    }
}