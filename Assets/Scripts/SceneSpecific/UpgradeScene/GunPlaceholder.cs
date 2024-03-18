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
        var gcr = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager.GunCurrentData;

        GBase gBase = gcr.Value.Spawn();

        gBase.name = "1";

        gBase.transform.SetParent(transform, false);
    }

    public void ReplacePart()
    {
        if (HoveredPart && PartBuilderInv.Selected)
        {

            var localPlayerGunManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager;
            if (HoveredPart.GetComponent<GUpgrade>())
            {
                UpgradeWithPart uwp = UpgradeManager.GetUpgradeById(PartBuilderInv.Selected.UpgradeId) as UpgradeWithPart;

                GUpgrade hoveredGU = HoveredPart.GetComponent<GUpgrade>();

                int hoverGUID = hoveredGU.UpgradeId;

                GUpgradeData gudNew = localPlayerGunManager.FindGUpgradeDataByUpId(PartBuilderInv.Selected.UpgradeId);
                GUpgradeData gudOld = localPlayerGunManager.FindGUpgradeDataByUpId(hoverGUID);



                if (gudNew == null ||
                    !(gudNew.UsedCount < gudNew.TotalCount) ||
                    gudOld == null// ||
                                  //!(gudOld.UsedCount > 0) REMOVE
                    )
                {
                    Debug.Log("one of requested are not in list!");
                    return;
                }
                //localPlayerGunManager.UnuseGUpgrade(hoverGUID); //decrease used count for the unused
                localPlayerGunManager.UseGUpgrade(PartBuilderInv.Selected.UpgradeId); //increase used count for the used


                hoveredGU.ReplacePart(uwp);
            }

            if (HoveredPart.GetComponent<GMuzzle>())
            {
                UpgradeWithPart uwp = UpgradeManager.GetUpgradeById(PartBuilderInv.Selected.UpgradeId) as UpgradeWithPart;
                GMuzzle hoveredGM = HoveredPart.GetComponent<GMuzzle>();
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

            }

            var selectedUpgrade = localPlayerGunManager.FindGUpgradeDataByUpId(PartBuilderInv.Selected.UpgradeId);
            if(selectedUpgrade.TotalCount <= selectedUpgrade.UsedCount)
            {
                PartBuilderInv.Selected = null;
                CursorHandler.Default();
            }

            PartBuilderInv.UpdateList();
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
        else
        {
            CursorHandler.Default();
        }

        var deleteBtnImage = _deleteBtn.GetComponent<Image>();
        if (deleteBtnImage)
            _deleteBtn.GetComponent<Image>().color = IsDelete ? Color.gray : Color.white;
    }

    public void InvPartClicked(PartBuilderInvChild invPart)
    {
        if (PartBuilderInv.Selected == invPart)
        {
            PartBuilderInv.Selected = null;
            CursorHandler.Default();
        }
        else
        {
            if (IsDelete)
            {
                ToggleDelete();
            }
            PartBuilderInv.Selected = invPart;
            CursorHandler.Hand();
        }
    }

    public void GunPartLogAsText()
    {
        Debug.Log(GunBaseSaveData.ParseToText(new GunBaseSaveData(transform.GetChild(0).GetComponent<GBase>()).Child));
    }
}
