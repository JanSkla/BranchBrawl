using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UpgradeWithPart : Upgrade
{
    protected static int _branchingCount = 1;

    private string _upgradePrefabResource;

    public UpgradeWithPart(int id, string upgradePrefabResource, string descripotion) : base(id, descripotion)
    {
        _upgradePrefabResource = upgradePrefabResource;
    }
    public override void OnAdd(PlayerManager pm)
    {
        pm.PlayerGunManager.AddGUpgrade(this);
        Debug.Log(Description + " upgrade added");
    }
    public override void OnDelete()
    {
        Debug.Log(Description + " upgrade deleted");
    }
    public GUpgrade InstantiatePrefab()
    {
        var prefab = Resources.Load(_upgradePrefabResource) as GameObject;
        var instance = Object.Instantiate(prefab);
        GUpgrade gu = instance.GetComponent<GUpgrade>();
        gu.UpgradeId = Id;
        return gu;
    }
    public abstract int GetBranchCount();
}
