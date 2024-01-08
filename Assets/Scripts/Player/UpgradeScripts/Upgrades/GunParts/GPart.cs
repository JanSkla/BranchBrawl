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
            if (GameObject.Find("GunPlaceholder").GetComponent<GunPlaceholder>().IsDelete && !GetComponent<GUpgrade>().IsUnityNull())
            {
                SetOutlineTotal(true);
            }
            else if(!GetComponent<GUpgrade>().IsUnityNull() || !GetComponent<GMuzzle>().IsUnityNull())
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
            SetOutlineTotal(false);
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
            if (GameObject.Find("GunPlaceholder").GetComponent<GunPlaceholder>().IsDelete && !GetComponent<GUpgrade>().IsUnityNull())
            {

                GPoint gp = transform.parent.GetComponent<GPoint>();

                GameObject parentparentGO = gp.parent;

                GDestiny parentGDestRef;

                if (parentparentGO.GetComponent<GUpgrade>())
                {
                    int di = gp.destinyIndex;

                    parentGDestRef = parentparentGO.GetComponent<GUpgrade>().Destiny[di];
                }
                else if (parentparentGO.GetComponent<GBase>())
                {
                    parentGDestRef = parentparentGO.GetComponent<GBase>().Destiny;
                }
                else
                {
                    Debug.Log("There is no GUpgrade nor GBase");
                    return;
                }
                //

                GameObject gMuzzleprefab = Resources.Load("Prefabs/GunParts/GMuzzle") as GameObject;
                GMuzzle gMuzzle = Instantiate(gMuzzleprefab).GetComponent<GMuzzle>();

                Debug.Log(gMuzzle);

                gMuzzle.transform.SetParent(parentGDestRef.Position, false);

                parentGDestRef.Part = gMuzzle;

                DestroyPartRecursive();
            }
            else if (!GetComponent<GUpgrade>().IsUnityNull() || !GetComponent<GMuzzle>().IsUnityNull())
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
