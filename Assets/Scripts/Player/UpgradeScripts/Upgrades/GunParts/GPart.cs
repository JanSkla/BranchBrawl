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

    public abstract void Shoot(ShootData shot);
    public void DestroyPartRecursive()
    {
        if (GameObject.Find("GunPlaceholder") != null)
        {
            var localPlayerGunManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager;
            AssignToInvR(GetComponent<GUpgrade>());
            void AssignToInvR(GUpgrade gu)
            {
                if (gu == null) return;

                localPlayerGunManager.UnuseGUpgrade(gu.UpgradeId);

                for (int i = 0; i < gu.Destiny.Length; i++)
                {
                    AssignToInvR(gu.Destiny[i].Part.GetComponent<GUpgrade>());
                }
            }
        }

        DestroyR(transform);
        void DestroyR(Transform t)
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

                GameObject parentparentGO = gp.Parent;

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
                    PlayerGunManager.NetworkMuzzleInstantiateOnDestiny(parentGDestRef);
                else
                    PlayerGunManager.MuzzleInstantiateOnDestiny(parentGDestRef);

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
