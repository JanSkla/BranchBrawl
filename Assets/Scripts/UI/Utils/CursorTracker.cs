using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTracker : MonoBehaviour
{
    [SerializeField]
    private Material _material;
    void Update()
    {
        if(_material)
            _material.SetVector("_MousePosition", Input.mousePosition);
    }
}
