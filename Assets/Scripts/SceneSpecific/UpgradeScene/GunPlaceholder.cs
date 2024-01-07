using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GunPlaceholder : MonoBehaviour
{
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

            HoveredPart.GetComponent<GUpgrade>().ReplacePart(uwp);
        }
    }
}
