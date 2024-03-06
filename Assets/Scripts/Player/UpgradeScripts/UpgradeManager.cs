using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private List<Upgrade> _ownedUpgrades = new();

    private static List<Upgrade> _upgradeTypes = new() {
        new UpgradeG2Splitter((int)Upgrades.G2Splitter),
        //new UpgradeGEmptyEnhancer(2),
        new UpgradeGChargeEnhancer((int)Upgrades.GChargeEnhancer),
        new UpgradeGConeEnhancer((int)Upgrades.GConeEnhancer),
        new UpgradeGFireEnhancer((int)Upgrades.GFireEnhancer),
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

    public static Upgrade[] GetRandomSetOfNonrepeatingUpgrades(int amount)
    {
        Upgrade[] value = new Upgrade[amount];

        List<int> unusedSpots = new List<int>();

        for (int i = 0; i < _upgradeTypes.Count; i++)
        {
            unusedSpots.Add(i);
        }
        for (int i = 0; i < amount; i++)
        {
            int random = Random.Range(0, unusedSpots.Count);
            int r = unusedSpots[random];
            value[i] = _upgradeTypes[r];
            unusedSpots.RemoveAt(random);
        }

        return value;
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        _ownedUpgrades.Add(upgrade);
        upgrade.OnAdd(GetComponent<PlayerManager>());
    }
}

public enum Upgrades
{
    G2Splitter = 1,
    GChargeEnhancer = 3,
    GConeEnhancer = 4,
    GFireEnhancer = 5
}
