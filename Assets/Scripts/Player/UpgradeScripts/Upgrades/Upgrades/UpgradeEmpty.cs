using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeEmpty : Upgrade
{
    public UpgradeEmpty(int id) : base(id, "Empty idk neco") { }
    public override void OnAdd(PlayerManager player)
    {
        Debug.Log("Empty upgrade added");
    }
    public override void OnDelete(PlayerManager player)
    {
        Debug.Log("Empty upgrade deleted");
    }
}
