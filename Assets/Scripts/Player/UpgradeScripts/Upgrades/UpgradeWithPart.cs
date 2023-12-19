using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradeWithPart : Upgrade
{
    public string UpgradePrefabResource;

    public UpgradeWithPart(int id, string upgradePrefabResource, string descripotion) : base(id, descripotion)
    {
        UpgradePrefabResource = upgradePrefabResource;
    }
}
