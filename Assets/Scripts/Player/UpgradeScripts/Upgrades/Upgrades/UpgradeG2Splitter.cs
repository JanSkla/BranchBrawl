using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeG2Splitter : UpgradeWithPart
{
    public UpgradeG2Splitter(int id) : base(id, "Prefabs/GunUpgrades/G2Splitter", "1 - 2 splitter") { }
    public override void OnAdd(PlayerManager pm)
    {
        pm.PlayerGunManager.AddGUpgrade(this);
        Debug.Log("split upgrade added");
    }
    public override void OnDelete()
    {
        Debug.Log("split upgrade deleted");
    }
}
