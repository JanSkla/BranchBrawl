using UnityEngine;

public abstract class GPart : MonoBehaviour
{
    public void Shoot(ShootData shot) { }
    public void DestroyPartRecursive()
    {
        DestroyR(transform);
        static void DestroyR(Transform t)
        {
            foreach (Transform child in t)
            {
                DestroyR(child);
            }
            Destroy(t);
        }
    }
}
