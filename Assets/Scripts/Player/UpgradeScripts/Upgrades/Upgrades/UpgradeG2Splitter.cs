using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeG2Splitter : Upgrade
{
    private GameObject upgradePrefab = Resources.Load("Prefabs/GunUpgrades/G2Splitter") as GameObject;
    public UpgradeG2Splitter(int id) : base(id, "1 - 2 splitter") { }
    public override void OnAdd(PlayerManager player)
    {
        Debug.Log("split upgrade added");
    }
    public override void OnDelete()
    {
        Debug.Log("split upgrade deleted");
    }
}
