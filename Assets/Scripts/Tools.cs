using System;
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

    public static IEnumerator SmoothLerpMoveTo(float originVal, float goalAmount, float duration, Action<float, float, float> action)
    {
        float timer = 0;
        float progress;
        while (timer < duration)
        {
            progress = timer / duration;
            progress = Mathf.Lerp(0, Mathf.PI, progress);
            progress = Mathf.Cos(progress);
            progress = progress / 2f + 0.5f;
            progress = 1 - progress;
            action(originVal, goalAmount, progress * progress);
            timer += Time.deltaTime;
            yield return null;
        }
        action(originVal, goalAmount, 1);
    }
}
