using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyUpgrade : Upgrade
{
    public EmptyUpgrade(int id) : base(id, "Empty idk neco") { }
    public override void OnAdd()
    {
        Debug.Log("Empty upgrade added");
    }
    public override void OnDelete()
    {
        Debug.Log("Empty upgrade deleted");
    }
}
