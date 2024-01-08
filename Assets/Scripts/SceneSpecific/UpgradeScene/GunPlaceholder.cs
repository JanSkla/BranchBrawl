using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GunPlaceholder : MonoBehaviour
{
    public bool IsDelete = false;
    public GPart HoveredPart;

    [SerializeField]
    private PartBuilderInv _partBuilderInv;

    // Start is called before the first frame update
    void Start()
    {
        var gcr =  NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager.GunCurrentData;

        GBase gBase = gcr.Value.Spawn();

        gBase.name = "1";

        gBase.transform.SetParent(transform, false);
    }

    public void ReplacePart()
    {
        if (HoveredPart && _partBuilderInv.Selected && HoveredPart.GetComponent<GUpgrade>())
        {
            UpgradeWithPart uwp = UpgradeManager.GetUpgradeById(_partBuilderInv.Selected.UpgradeId) as UpgradeWithPart;

            GUpgrade hoveredGU = HoveredPart.GetComponent<GUpgrade>();

            int hoverGUID = hoveredGU.UpgradeId;
            var localPlayerGunManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager;

            GUpgradeData gudNew = localPlayerGunManager.FindGUpgradeDataByUpId(_partBuilderInv.Selected.UpgradeId);
            GUpgradeData gudOld = localPlayerGunManager.FindGUpgradeDataByUpId(hoverGUID);


            Debug.Log(!(gudNew.UsedCount < gudNew.TotalCount));
            //Debug.Log(!(gudOld.UsedCount > 0));  // TEST REMOVE
            Debug.Log(gudNew == null);
            Debug.Log(gudOld == null);

            if (gudNew == null ||
                !(gudNew.UsedCount < gudNew.TotalCount) ||
                gudOld == null// ||
                //!(gudOld.UsedCount > 0) REMOVE
                )
            {
                Debug.Log("one of requested are not in list!");
                return;
            }
            localPlayerGunManager.UseGUpgrade(_partBuilderInv.Selected.UpgradeId); //increase used count for the used
            localPlayerGunManager.UnuseGUpgrade(hoverGUID); //decrease used count for the unused

            _partBuilderInv.UpdateList();

            hoveredGU.ReplacePart(uwp);

            _partBuilderInv.Selected.SetSelected(false);
        }

        if (HoveredPart && _partBuilderInv.Selected && HoveredPart.GetComponent<GMuzzle>())
        {
            UpgradeWithPart uwp = UpgradeManager.GetUpgradeById(_partBuilderInv.Selected.UpgradeId) as UpgradeWithPart;
            GMuzzle hoveredGM = HoveredPart.GetComponent<GMuzzle>();

            hoveredGM.ReplaceMuzzle(uwp);

            var localPlayerGunManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager;
            GUpgradeData gudNew = localPlayerGunManager.FindGUpgradeDataByUpId(_partBuilderInv.Selected.UpgradeId);

            if (gudNew == null ||
                !(gudNew.UsedCount < gudNew.TotalCount)
                )
            {
                Debug.Log("one of requested are not in list!");
                return;
            }

            localPlayerGunManager.UseGUpgrade(_partBuilderInv.Selected.UpgradeId); //increase used count for the used

            _partBuilderInv.UpdateList();

            _partBuilderInv.Selected.SetSelected(false);
            IsDelete = false;
        }
    }

    public void ToggleDelete()
    {
        if (IsDelete)
        {
            _partBuilderInv.Selected = null;
        }
        IsDelete = !IsDelete;
    }
}
