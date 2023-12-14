using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasSource
{
    public IHasDestiny Source { get; set; }
    public void Shoot(int amount);
}
