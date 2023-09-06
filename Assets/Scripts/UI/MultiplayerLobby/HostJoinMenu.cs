using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostJoinMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _mpLobby;
    public void OnHostClick()
    {
        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
        _mpLobby.gameObject.SetActive(true);
    }

    public void OnJoinClick()
    {
        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
        _mpLobby.gameObject.SetActive(true);
    }
}
