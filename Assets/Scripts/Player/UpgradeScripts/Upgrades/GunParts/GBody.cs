using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBody : MonoBehaviour, IHasDestiny
{
    private IHasSource[] _destiny;
    public IHasSource[] Destiny
    {
        get { return _destiny; }
        set { _destiny = value; }
    }
    public void Shoot() { }
}
