using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private List<Upgrade> _ownedUpgrades = new();

    private static List<Upgrade> _upgradeTypes = new() {
        new UpgradeG2Splitter(1),
        //new UpgradeGEmptyEnhancer(2),
        new UpgradeGChargeEnhancer(3),
        new UpgradeGConeEnhancer(4),
        new UpgradeGFireEnhancer(5),
    };

    public static Upgrade GetUpgradeById(int id)
    {
        Upgrade u = _upgradeTypes.Find(e => e.Id == id);
        return u;
    }

    public static Upgrade GetRandomUpgrade()
    {
        int total = _upgradeTypes.Count;
        int r = Random.Range(0, total);
        return _upgradeTypes[r];
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        _ownedUpgrades.Add(upgrade);
        upgrade.OnAdd(GetComponent<PlayerManager>());
    }
}
