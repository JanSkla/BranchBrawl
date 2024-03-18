using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UpgradeWithPart : Upgrade
{
    protected static int _branchingCount = 1;

    public UpgradeWithPart(int id, string name) : base(id, name)
    {
    }
    public override void OnAdd(PlayerManager pm)
    {
        pm.PlayerGunManager.AddGUpgrade(Id);
        Debug.Log(Name + " upgrade added");
    }
    public override void OnDelete(PlayerManager pm)
    {
        pm.PlayerGunManager.RemoveGUpgrade(Id);
        Debug.Log(Name + " upgrade deleted");
    }
    public GUpgrade InstantiatePrefab()
    {
        var prefab = Resources.Load("Prefabs/GunUpgrades/" + Name) as GameObject;
        var instance = Object.Instantiate(prefab);
        GUpgrade gu = instance.GetComponent<GUpgrade>();
        gu.UpgradeId = Id;
        return gu;
    }
    public abstract int GetBranchCount();
}
