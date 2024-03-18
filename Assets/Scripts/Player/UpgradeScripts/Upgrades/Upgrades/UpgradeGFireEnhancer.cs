using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeGFireEnhancer : UpgradeWithPart
{
    protected new static int _branchingCount = 1;
    public UpgradeGFireEnhancer(int id) : base(id, "GFireEnhancer") { }
    public override int GetBranchCount()
    {
        return _branchingCount;
    }
}