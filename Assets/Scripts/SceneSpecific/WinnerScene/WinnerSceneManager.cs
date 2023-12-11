using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinnerSceneManager : MonoBehaviour
{
    private GameManager _gameManager;
    private NetworkData _networkData;

    [SerializeField]
    private GameObject _winnerList;

    [SerializeField]
    private GameObject _tabRowPrefab;
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _networkData = GameObject.Find("NetworkDataManager(Clone)").GetComponent<NetworkData>();
        _gameManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();

        foreach (Transform child in _winnerList.transform)
        {
            Destroy(child.gameObject);
        }

        List<PlayerGameData> sortedList = new();

        foreach (var data in _gameManager.PlayersGameData)
        {
            sortedList.Add(data);
        }

        sortedList = sortedList.OrderBy(o => o.Crowns).Reverse().ToList();

        for (int i = 0; i < sortedList.Count; i++)
        {
            var data = sortedList[i];
            var pm = NetworkManager.Singleton.SpawnManager.SpawnedObjects[data.PMNwId].GetComponent<PlayerManager>();
            AddRow(i + 1 + ". " + pm.PlayerName.Value.ToString(), data.Crowns);
        }
    }

    private void AddRow(string name, int crowns)
    {
        var tabRow = Instantiate(_tabRowPrefab, _winnerList.transform).GetComponent<TabRow>();
        tabRow.PlayerName.text = name;
        tabRow.CrownCount.text = crowns + "x";
    }
    // Start is called before the first frame update
    public void StarNewGame()
    {
        _networkData.StartNewGame();
    }
    public void GoToLobby()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("MultiplayerLobby", LoadSceneMode.Single);
    }
}
