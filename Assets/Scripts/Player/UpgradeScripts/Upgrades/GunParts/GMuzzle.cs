using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMuzzle : MonoBehaviour, IHasSource
{
    private IHasDestiny _source;
    public IHasDestiny Source
    {
        get { return _source; }
        set { _source = value; }
    }
    public void Shoot(int amount)
    {
        Debug.Log("Shoots successfully");
    }
}
