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
        if (NetworkManager.IsHost)
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
        NetworkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
