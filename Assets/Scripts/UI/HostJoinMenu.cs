using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostJoinMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _inGameUI;
    public void OnHostClick()
    {
        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
        _inGameUI.SetActive(true);
    }

    public void OnJoinClick()
    {
        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
        _inGameUI.SetActive(true);
    }
}
