using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(DestroySelf), 1);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
