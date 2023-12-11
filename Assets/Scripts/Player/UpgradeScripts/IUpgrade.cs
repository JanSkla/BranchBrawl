using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgrade
{
    string Name { get; }
    void OnAdd();
    void OnDelete();
}
