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
    public GameObject InstantiatePrefab()
    {
        var prefab = Resources.Load(_upgradePrefabResource) as GameObject;
        var instance = Object.Instantiate(prefab);
        instance.GetComponent<GUpgrade>().UpgradeId = Id;
        return instance;
    }
    public int GetBranchingCount()
    {
        return _branchingCount;
    }
}
