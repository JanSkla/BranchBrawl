using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class GPart : NetworkBehaviour
{
    [SerializeField]
    private Outline _partOutline;
    [SerializeField]
    private Outline _totalOutline;

    void Start()
    {
        if(_partOutline)
            _partOutline.enabled = false;
        if (_totalOutline)
            _totalOutline.enabled = false;
    }

    public abstract void Shoot(ShootData shot, Player owner);
    public void DestroyPartRecursive()
    {
        Debug.Log("AA");
        if (GameObject.Find("GunPlaceholder") != null)
        {
            var localPlayerGunManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager;
            AssignToInvR(GetComponent<GUpgrade>());
            void AssignToInvR(GUpgrade gu)
            {
                Debug.Log("AAAA");
                if (gu == null) return;

                localPlayerGunManager.UnuseGUpgrade(gu.UpgradeId);

                for (int i = 0; i < gu.Destiny.Length; i++)
                {
                    if(gu.Destiny[i].Part)
                        AssignToInvR(gu.Destiny[i].Part.GetComponent<GUpgrade>());
                }
            }
        }

        Utils.DestroyWithChildren(transform.gameObject);
        //void DestroyR(Transform t)
        //{
        //    foreach (Transform child in t)
        //    {
        //        DestroyR(child);
        //    }
        //    Destroy(t.gameObject);
        //}
    }

    void OnMouseOver()
    {
        if (GameObject.Find("GunPlaceholder") != null)
        {
            if (GameObject.Find("GunPlaceholder").GetComponent<GunPlaceholder>().IsDelete && !GetComponent<GUpgrade>().IsUnityNull())
            {
                if (_totalOutline)
                    SetOutlineTotal(true);
            }
            else if (!GetComponent<GUpgrade>().IsUnityNull() || !GetComponent<GMuzzle>().IsUnityNull())
            {
                GameObject.Find("GunPlaceholder").GetComponent<GunPlaceholder>().HoveredPart = this;
                if (_partOutline)
                    SetOutlinePart(true);
            }
        }
    }
    void OnMouseExit()
    {
        if (GameObject.Find("GunPlaceholder") != null)
        {
            if (_totalOutline)
                SetOutlineTotal(false);
            if (_partOutline)
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

                GameObject parentparentGO = gp.Parent.gameObject;

                GDestiny parentGDestRef;

                if (parentparentGO.GetComponent<GUpgrade>())
                {
                    int di = gp.DestinyIndex;

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

                if (NetworkObject.IsSpawned)
                    MuzzleManager.NetworkMuzzleInstantiateOnDestiny(parentGDestRef);
                else
                    MuzzleManager.MuzzleInstantiateOnDestiny(parentGDestRef);

                DestroyPartRecursive();
                GameObject.Find("GunPlaceholder").GetComponent<GunPlaceholder>().PartBuilderInv.UpdateList();
            }
            else if (!GetComponent<GUpgrade>().IsUnityNull() || !GetComponent<GMuzzle>().IsUnityNull())
            {
                GameObject.Find("GunPlaceholder").GetComponent<GunPlaceholder>().ReplacePart();
            }
        }
    }

    public void SetOutlineTotal(bool isOutline)
    {
        if (isOutline)
        {
            _totalOutline.enabled = true;
            _totalOutline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        }
        else
        {
            _totalOutline.enabled = false;
        }
    }
    public void SetOutlinePart(bool isOutline)
    {
        if (isOutline)
        {
            _partOutline.enabled = true;
            _partOutline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        }
        else
        {
            _partOutline.enabled = false;
        }
    }


}
