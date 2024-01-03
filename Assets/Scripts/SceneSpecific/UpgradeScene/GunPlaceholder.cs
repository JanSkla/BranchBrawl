using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GunPlaceholder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var gcr =  NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager.GunCurrentData;

        GBase gBase = gcr.Value.Spawn();

        gBase.name = "1";

        gBase.transform.SetParent(transform, false);
    }
}
