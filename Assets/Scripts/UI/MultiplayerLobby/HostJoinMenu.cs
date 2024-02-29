using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine.SceneManagement;

public class HostJoinMenu : MonoBehaviour
{
    [SerializeField]
    private int m_MaxConnections = 5; //huh

    [SerializeField]
    private GameObject loadingView;

    [SerializeField]
    private TMP_InputField joinInput;

    [SerializeField]
    private GameObject _networkManagerPrefab;
    [SerializeField]
    private GameObject _networkDataManagerPrefab;

    void Start()
    {

        if (!GameObject.Find("NetworkManager"))
        {
            var nm = Instantiate(_networkManagerPrefab);
            nm.name = "NetworkManager";
        }
        gameObject.SetActive(false);
    }

    public void OnHostClick()
    {
        StartCoroutine(Example_ConfigureTransportAndStartNgoAsHost());
        //NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
        loadingView.SetActive(true);
    }

    public void OnJoinClick()
    {
        if(joinInput.text != null)
        {
            Debug.Log($"Tries to connect with code: {joinInput.text}");
            StartCoroutine(Example_ConfigureTransportAndStartNgoAsConnectingPlayer(joinInput.text));
        }
        //NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
        loadingView.SetActive(true);
    }

    //Host
    public static async Task<RelayServerData> AllocateRelayServerAndGetJoinCode(int maxConnections, GameObject loadingView, GameObject networkDataManagerPrefab, string region = null)
    {
        Allocation allocation;
        string createJoinCode;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
        }
        catch (Exception e)
        {
            loadingView.SetActive(false);
            Debug.LogError($"Relay create allocation request failed {e.Message}");
            throw;
        }

        Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"server: {allocation.AllocationId}");

        try
        {
            createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Host started with code " + createJoinCode);
        }
        catch
        {
            loadingView.SetActive(false);
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        Debug.Log($"serverstarted");

        RelayServerData relaySeverData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relaySeverData);
        NetworkManager.Singleton.StartHost();

        GameObject networkDataManager = Instantiate(networkDataManagerPrefab);
        networkDataManager.GetComponent<NetworkObject>().Spawn(false);
        networkDataManager.GetComponent<NetworkData>().JoinCode.Value = createJoinCode;
        NetworkManager.Singleton.SceneManager.LoadScene("MultiplayerLobby", LoadSceneMode.Single);

        return relaySeverData;
    }

    IEnumerator Example_ConfigureTransportAndStartNgoAsHost()
    {
        var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(m_MaxConnections, loadingView, _networkDataManagerPrefab);

        while (!serverRelayUtilityTask.IsCompleted)
        {
            yield return null;
        }
    }

    //Client
    public static async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinCode, GameObject loadingView)
    {
        JoinAllocation allocation;
        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            loadingView.SetActive(false);
            throw;
        }

        Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"client: {allocation.AllocationId}");

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();

        return relayServerData;
    }

    IEnumerator Example_ConfigureTransportAndStartNgoAsConnectingPlayer(string RelayJoinCode)
    {
        // Populate RelayJoinCode beforehand through the UI
        var clientRelayUtilityTask = JoinRelayServerFromJoinCode(RelayJoinCode, loadingView);

        while (!clientRelayUtilityTask.IsCompleted)
        {
            yield return null;
        }
    }
}
