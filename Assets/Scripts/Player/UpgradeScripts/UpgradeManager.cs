using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private List<Upgrade> upgrades = new();

    private static List<Upgrade> upgradeTypes = new() {
        new UpgradeG2Splitter(1),
        new UpgradeG2Splitter(2),
    };

    public static Upgrade GetUpgradeById(int id)
    {
        Upgrade u = upgradeTypes.Find(e => e.Id == id);
        return u;
    }

    public static Upgrade GetRandomUpgrade()
    {
        int total = upgradeTypes.Count;
        int r = Random.Range(0, total);
        return upgradeTypes[r];
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        upgrades.Add(upgrade);
        upgrade.OnAdd(GetComponent<PlayerManager>());
    }
}
