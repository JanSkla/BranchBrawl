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
    public UpgradeCard InstantiateSelectionCard(Action<int> upgradeSelected)
    {
        var upgradeCardprefab = Resources.Load("Prefabs/GunUpgrades/UpgradeCard") as GameObject;
        var newCard = Object.Instantiate(upgradeCardprefab);
        UpgradeCard card = newCard.GetComponent<UpgradeCard>();

        card.Image.sprite = Resources.Load<Sprite>("Images/Upgrades/" + Name + "/Image");
        card.Icon.sprite = Resources.Load<Sprite>("Images/Upgrades/" + Name + "/Icon");
        card.Button.onClick.AddListener(() => upgradeSelected.Invoke(Id));
        card.Name.text = Name;
        return card;
    }
}
