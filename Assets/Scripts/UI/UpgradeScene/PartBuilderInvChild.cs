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
    public int Count;

    public PartBuilderInv PartBuilderInv;
    //obj ref
    public TextMeshProUGUI CountText;
    public Image Icon;

    public void SetGUpgradeData(GUpgradeData gud) //init called by PartBuilderInv on being Instanced
    {
        UpgradeId = gud.UpgradeId;
        Count = gud.TotalCount - gud.UsedCount;
        CountText.text = Count + "x";
        Icon.sprite = UpgradeManager.GetUpgradeById(gud.UpgradeId).GetIconSprite();

        GetComponent<Button>().onClick.AddListener(OnClick);
    }


    private void OnClick()
    {
        PartBuilderInv.GunPlaceholder.InvPartClicked(this);
    }
}