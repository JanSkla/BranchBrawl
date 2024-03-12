using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerSetup : MonoBehaviour
{
    void Start()
    {
        AuthenticatingAPlayer();

        NetworkManager.Singleton.OnClientStopped += OnClientStopped;
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
    }

    private void OnClientStopped(bool isHost)
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }

    async void AuthenticatingAPlayer()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var playerID = AuthenticationService.Instance.PlayerId;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
