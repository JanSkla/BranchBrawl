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
    private GameObject _loading;
    [SerializeField]
    private GameObject _hostView;
    [SerializeField]
    private GameObject _clientView;
    [SerializeField]
    private TextMeshProUGUI _joinCodeDisplay;
    [SerializeField]
    private PlayerStandPlacehodlerManager _playerStandPlacehodlerManager;

    private NetworkData _networkDataManager;
    void Start()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            _clientView.SetActive(false);
            _hostView.SetActive(true);
        }
        else
        {
            _hostView.SetActive(false);
            _clientView.SetActive(true);
        }
        _networkDataManager = GameObject.Find("NetworkDataManager(Clone)").GetComponent<NetworkData>();
        _joinCodeDisplay.text = _networkDataManager.JoinCode.Value.ToString();
        _networkDataManager.PlayerObjectNwIds.OnListChanged += OnPlayerNwIdsChange;


        ulong[] list = new ulong[_networkDataManager.PlayerObjectNwIds.Count];

        for (int i = 0; i < _networkDataManager.PlayerObjectNwIds.Count; i++)
        {
            list[i] = _networkDataManager.PlayerObjectNwIds[i];
        }

        _playerStandPlacehodlerManager.RerenderCurrentPlayers(list);
    }
    private void OnDisable()
    {
        SetLoading();
    }
    private void OnDestroy()
    {
        _networkDataManager.PlayerObjectNwIds.OnListChanged -= OnPlayerNwIdsChange;
    }
    public void SetLoading()
    {
        _loading.SetActive(true);
    }
    public void StartGame()
    {
        _networkDataManager.StartNewGame();
        //gminstance.GetComponent<GameManager>().StartGame();
    }
    public void GoBack()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }
    private void OnPlayerNwIdsChange(NetworkListEvent<ulong> changeEvent)
    {
        Debug.Log("AA" + _networkDataManager.PlayerObjectNwIds.Count);
        ulong[] list = new ulong[_networkDataManager.PlayerObjectNwIds.Count];

        for (int i = 0; i < _networkDataManager.PlayerObjectNwIds.Count; i++)
        {
            list[i] = _networkDataManager.PlayerObjectNwIds[i];
        }

        _playerStandPlacehodlerManager.RerenderCurrentPlayers(list);
    }
    public void SetCombatSceneMapName(string sceneName)
    {
        _networkDataManager.CombatRoundSceneName = sceneName;
    }
}
