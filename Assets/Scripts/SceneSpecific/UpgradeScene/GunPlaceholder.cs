using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GunPlaceholder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager.GunCurrentData.Spawn();
    }
}
