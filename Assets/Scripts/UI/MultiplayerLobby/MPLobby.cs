using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MPLobby : NetworkBehaviour
{
    public void StartGame()
    {
        NetworkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
