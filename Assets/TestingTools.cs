using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestingTools : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    void Start()
    {
        gameObject.SetActive(NetworkManager.Singleton.IsServer);
    }

    public void PlayAgain()
    {
        gameManager.PlayAgain();
    }
}
