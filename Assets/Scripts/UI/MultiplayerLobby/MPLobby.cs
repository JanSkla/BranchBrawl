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
    [SerializeField]
    private GameObject gameManager;
    void Start()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            clientView.SetActive(false);
            hostView.SetActive(true);
        }
        else
        {
            hostView.SetActive(false);
            clientView.SetActive(true);
        }
    }
    public void StartGame()
    {
        GameObject gminstance =  Instantiate(gameManager);
        gminstance.GetComponent<GameManager>().StartGame();
    }
    public void GoBack()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MultiplayerLobby");
    }
}
