using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostJoinMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject InGameUI;
    public void OnHostClick()
    {
        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
        InGameUI.SetActive(true);
    }

    public void OnJoinClick()
    {
        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
        InGameUI.SetActive(true);
    }
}
