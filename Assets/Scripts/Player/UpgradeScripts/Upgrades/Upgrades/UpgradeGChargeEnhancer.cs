using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeGChargeEnhancer : UpgradeWithPart
{
    protected new static int _branchingCount = 1;
    public UpgradeGChargeEnhancer(int id) : base(id, "GChargeEnhancer") { }
    public override int GetBranchCount()
    {
        return _branchingCount;
    }
}
