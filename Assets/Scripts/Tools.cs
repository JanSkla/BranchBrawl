using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static void ChangeLayerWithChildren(GameObject gameObject, LayerMask layerMask)
    {
        gameObject.layer = layerMask;
        foreach (Transform child in gameObject.transform)
        {
            ChangeLayerWithChildren(child.gameObject, layerMask);
        }
    }
}
