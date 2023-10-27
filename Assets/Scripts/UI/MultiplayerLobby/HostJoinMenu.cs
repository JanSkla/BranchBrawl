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

public class HostJoinMenu : MonoBehaviour
{
    [SerializeField]
    private int m_MaxConnections = 5; //huh

    [SerializeField]
    private GameObject mpLobby;

    [SerializeField]
    private GameObject loadingView;

    [SerializeField]
    private TMP_InputField joinInput;

    [SerializeField]
    private TextMeshProUGUI hostCodeDisplay;

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
    public static async Task<RelayServerData> AllocateRelayServerAndGetJoinCode(int maxConnections, TextMeshProUGUI hostCodeDisplay, GameObject mpLobbyView, GameObject loadingView, string region = null)
    {
        Allocation allocation;
        string createJoinCode;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
        }
        catch (Exception e)
        {
            Debug.LogError($"Relay create allocation request failed {e.Message}");
            throw;
        }

        Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"server: {allocation.AllocationId}");

        try
        {
            createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Host started with code " + createJoinCode);
            hostCodeDisplay.text = createJoinCode;
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        Debug.Log($"serverstarted");

        RelayServerData relaySeverData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relaySeverData);
        NetworkManager.Singleton.StartHost();

        loadingView.SetActive(false);
        mpLobbyView.SetActive(true);

        return relaySeverData;
    }

    IEnumerator Example_ConfigureTransportAndStartNgoAsHost()
    {
        hostCodeDisplay.text = "getting join code";
        var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(m_MaxConnections, hostCodeDisplay, mpLobby, loadingView);

        while (!serverRelayUtilityTask.IsCompleted)
        {
            yield return null;
        }
    }

    //Client
    public static async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinCode, GameObject mpLobbyView, GameObject loadingView)
    {
        JoinAllocation allocation;
        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"client: {allocation.AllocationId}");

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();

        loadingView.SetActive(false);
        mpLobbyView.SetActive(true);

        return relayServerData;
    }

    IEnumerator Example_ConfigureTransportAndStartNgoAsConnectingPlayer(string RelayJoinCode)
    {
        // Populate RelayJoinCode beforehand through the UI
        var clientRelayUtilityTask = JoinRelayServerFromJoinCode(RelayJoinCode, mpLobby, loadingView);

        while (!clientRelayUtilityTask.IsCompleted)
        {
            yield return null;
        }
    }
}
