using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeG2Splitter : UpgradeWithPart
{
    protected new static int _branchingCount = 2;
    public UpgradeG2Splitter(int id) : base(id, "Prefabs/GunUpgrades/G2Splitter", "1 - 2 splitter") { }
    public override int GetBranchCount()
    {
        return 2;
    }
}
