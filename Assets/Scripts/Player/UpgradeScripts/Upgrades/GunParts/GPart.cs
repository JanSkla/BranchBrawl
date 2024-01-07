using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class GPart : MonoBehaviour
{
    [SerializeField]
    private Outline _partOutline;
    [SerializeField]
    private Outline _totalOutline;

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
            Destroy(t.gameObject);
        }
    }

    void OnMouseOver()
    {
        if (GameObject.Find("GunPlaceholder") != null)
        {
            if(!GetComponent<GUpgrade>().IsUnityNull() || !GetComponent<GMuzzle>().IsUnityNull())
            {
                GameObject.Find("GunPlaceholder").GetComponent<GunPlaceholder>().HoveredPart = this;
                SetOutlinePart(true);
            }
        }
    }
    void OnMouseExit()
    {
        if (GameObject.Find("GunPlaceholder") != null)
        {
            SetOutlinePart(false);
            if (!GetComponent<GUpgrade>().IsUnityNull() || !GetComponent<GMuzzle>().IsUnityNull())
            {
                GameObject.Find("GunPlaceholder").GetComponent<GunPlaceholder>().HoveredPart = null;
            }
        }
    }
    void OnMouseDown()
    {
        if (GameObject.Find("GunPlaceholder") != null)
        {
            if (!GetComponent<GUpgrade>().IsUnityNull() || !GetComponent<GMuzzle>().IsUnityNull())
            {
                GameObject.Find("GunPlaceholder").GetComponent<GunPlaceholder>().ReplacePart();
            }
        }
    }

    public void SetOutlineTotal(bool isOutline)
    {
        _totalOutline.OutlineMode = isOutline ? Outline.Mode.OutlineVisible : Outline.Mode.OutlineHidden;
    }
    public void SetOutlinePart(bool isOutline)
    {
        _partOutline.OutlineMode = isOutline ? Outline.Mode.OutlineVisible : Outline.Mode.OutlineHidden;
    }


}
