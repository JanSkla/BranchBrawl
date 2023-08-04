using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostJoinMenu : MonoBehaviour
{
    void Start()
    {
    }
    public void OnHostClick()
    {
        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
    }

    public void OnJoinClick()
    {
        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
    }
}
