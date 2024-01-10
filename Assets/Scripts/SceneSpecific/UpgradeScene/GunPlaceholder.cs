using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GunPlaceholder : MonoBehaviour
{
    public bool IsDelete = false;
    public GPart HoveredPart;

    [SerializeField]
    private Button _deleteBtn;
    [SerializeField]
    public PartBuilderInv PartBuilderInv;

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
        if (HoveredPart && PartBuilderInv.Selected && HoveredPart.GetComponent<GUpgrade>())
        {
            UpgradeWithPart uwp = UpgradeManager.GetUpgradeById(PartBuilderInv.Selected.UpgradeId) as UpgradeWithPart;

            GUpgrade hoveredGU = HoveredPart.GetComponent<GUpgrade>();

            int hoverGUID = hoveredGU.UpgradeId;
            var localPlayerGunManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager;

            GUpgradeData gudNew = localPlayerGunManager.FindGUpgradeDataByUpId(PartBuilderInv.Selected.UpgradeId);
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
            localPlayerGunManager.UseGUpgrade(PartBuilderInv.Selected.UpgradeId); //increase used count for the used
            localPlayerGunManager.UnuseGUpgrade(hoverGUID); //decrease used count for the unused

            PartBuilderInv.UpdateList();

            hoveredGU.ReplacePart(uwp);

            PartBuilderInv.Selected.SetSelected(false);
        }

        if (HoveredPart && PartBuilderInv.Selected && HoveredPart.GetComponent<GMuzzle>())
        {
            UpgradeWithPart uwp = UpgradeManager.GetUpgradeById(PartBuilderInv.Selected.UpgradeId) as UpgradeWithPart;
            GMuzzle hoveredGM = HoveredPart.GetComponent<GMuzzle>();

            var localPlayerGunManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager;
            GUpgradeData gudNew = localPlayerGunManager.FindGUpgradeDataByUpId(PartBuilderInv.Selected.UpgradeId);

            if (gudNew == null ||
                !(gudNew.UsedCount < gudNew.TotalCount)
                )
            {
                Debug.Log("one of requested are not in list!");
                return;
            }


            hoveredGM.ReplaceMuzzle(uwp);

            localPlayerGunManager.UseGUpgrade(PartBuilderInv.Selected.UpgradeId); //increase used count for the used

            PartBuilderInv.UpdateList();

            PartBuilderInv.Selected.SetSelected(false);
            IsDelete = false;
        }
    }

    public void ToggleDelete()
    {
        IsDelete = !IsDelete;
        if (IsDelete)
        {
            PartBuilderInv.Selected = null;
            CursorHandler.Cross();
        }

        _deleteBtn.GetComponent<Image>().color = IsDelete ? Color.gray : Color.white;
    }

    public void GunPartLogAsText()
    {
        Debug.Log( PlayerGunManager.GunBaseSaveData.ParseToText(new PlayerGunManager.GunBaseSaveData(transform.GetChild(0).GetComponent<GBase>()).Child) );
    }
}
