using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade
{
    public int Id;
    public string Description;

    public Upgrade(int id, string description)
    {
        Id = id;
        Description = description;
    }
    public abstract void OnAdd(PlayerManager player);
    public abstract void OnDelete(PlayerManager player);
}
