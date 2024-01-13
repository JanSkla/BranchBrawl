using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeGEmptyEnhancer : UpgradeWithPart
{
    protected new static int _branchingCount = 1;
    public UpgradeGEmptyEnhancer(int id) : base(id, "Prefabs/GunUpgrades/GEmptyEnhancer", "1 - 1 enhancer") { }
    public override int GetBranchCount()
    {
        return 1;
    }
}
