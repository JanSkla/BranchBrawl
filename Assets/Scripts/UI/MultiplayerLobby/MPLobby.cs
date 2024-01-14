using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MPLobby : NetworkBehaviour
{
    [SerializeField]
    private GameObject hostView;
    [SerializeField]
    private GameObject clientView;
    [SerializeField]
    private TextMeshProUGUI _joinCodeDisplay;

    private GameObject _networkDataManager;
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
        _networkDataManager = GameObject.Find("NetworkDataManager(Clone)");
        _joinCodeDisplay.text = _networkDataManager.GetComponent<NetworkData>().JoinCode.Value.ToString();
    }
    public void StartGame()
    {
        _networkDataManager.GetComponent<NetworkData>().StartNewGame();
        //gminstance.GetComponent<GameManager>().StartGame();
    }
    public void GoBack()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }
}
