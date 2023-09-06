using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private NetworkObject _localPlayer;
    // Start is called before the first frame update
    void Start()
    {
        _localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
        _localPlayer.transform.Find("Head").GetComponent<PlayerCamera>().OnGameStart();
    }
}
