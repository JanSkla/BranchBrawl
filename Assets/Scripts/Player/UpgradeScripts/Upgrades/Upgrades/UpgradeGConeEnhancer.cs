using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeGConeEnhancer : UpgradeWithPart
{
    protected new static int _branchingCount = 1;
    public UpgradeGConeEnhancer(int id) : base(id, "GConeEnhancer") { }
    public override int GetBranchCount()
    {
        return _branchingCount;
    }
}
