using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : IUpgrade
{

    private string _name;
    public string Name
    {
        get { return _name; }
        set { _name = Name; }
    }
    public Upgrade(string name)
    {
        Name = name;
    }
    public void OnAdd()
    {

    }
    public void OnDelete()
    {

    }
}
