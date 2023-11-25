using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestingTools : MonoBehaviour
{
    [SerializeField]
    private RoundManager roundManager;

    void Start()
    {
        gameObject.SetActive(NetworkManager.Singleton.IsServer);
    }

    public void PlayAgain()
    {
        roundManager.PlayAgain();
    }
}
