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
    private GameObject gameManager;
    [SerializeField]
    private TextMeshProUGUI _joinCodeDisplay;
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
        var nwDM = GameObject.Find("NetworkDataManager(Clone)");
        Debug.Log(nwDM);
        Debug.Log(nwDM.GetComponent<NetworkData>());
        Debug.Log(nwDM.GetComponent<NetworkData>().JoinCode);
        Debug.Log(nwDM.GetComponent<NetworkData>().JoinCode.Value);
        _joinCodeDisplay.text = nwDM.GetComponent<NetworkData>().JoinCode.Value.ToString();
    }
    public void StartGame()
    {
        GameObject gminstance =  Instantiate(gameManager);
        //gminstance.GetComponent<GameManager>().StartGame();
    }
    public void GoBack()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("HostJoinMenu");
    }
}
