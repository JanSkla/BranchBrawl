using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MPLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject hostView;
    [SerializeField]
    private GameObject clientView;
    [SerializeField]
    private NetworkManager nwManager;
    void Start()
    {
        if (nwManager.IsHost || nwManager.IsServer)
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
        nwManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
