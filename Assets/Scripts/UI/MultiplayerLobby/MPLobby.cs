using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MPLobby : NetworkBehaviour
{
    [SerializeField]
    private GameObject hostView;
    [SerializeField]
    private GameObject clientView;
    void Start()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            hostView.SetActive(true);
        }
        else
        {
            clientView.SetActive(true);
        }
    }
    public void StartGame()
    {
        Debug.Log(NetworkManager.Singleton);
        Debug.Log(NetworkManager.Singleton.SceneManager);
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
