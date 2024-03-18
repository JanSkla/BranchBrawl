using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Upgrade
{
    public int Id;
    public string Name;

    public Upgrade(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public abstract void OnAdd(PlayerManager player);
    public abstract void OnDelete(PlayerManager player);
    public UpgradeCard InstantiateSelectionCard(Action<int> upgradeSelected, int optionIdx)
    {
        var upgradeCardprefab = Resources.Load("Prefabs/GunUpgrades/UpgradeCard") as GameObject;
        var newCard = Object.Instantiate(upgradeCardprefab);
        UpgradeCard card = newCard.GetComponent<UpgradeCard>();

        card.Image.sprite = GetImageSprite();
        card.Icon.sprite = GetIconSprite();
        card.Button.onClick.AddListener(() => upgradeSelected.Invoke(optionIdx));
        card.Name.text = Name;
        return card;
    }
    public Sprite GetImageSprite()
    {
        return Resources.Load<Sprite>("Images/Upgrades/" + Name + "/Image"); ;
    }
    public Sprite GetIconSprite()
    {
        return Resources.Load<Sprite>("Images/Upgrades/" + Name + "/Icon"); ;
    }
}
