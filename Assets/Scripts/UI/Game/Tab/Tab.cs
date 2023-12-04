using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Tab : MonoBehaviour
{
    [SerializeField]
    private GameObject _tabContainer;
    [SerializeField]
    private GameObject _tabRowPrefab;

    private GameManager _gameManager;

    void OnEnable()
    {
        _gameManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();
        foreach (Transform child in _tabContainer.transform)
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
            var pm = NetworkManager.Singleton.ConnectedClients[data.ClientId].PlayerObject.GetComponent<PlayerManager>();
            AddRow(pm.PlayerName.Value.ToString(), data.Crowns);
        }
    }

    private void AddRow(string name, int crowns)
    {
        var tabRow = Instantiate(_tabRowPrefab, _tabContainer.transform).GetComponent<TabRow>();
        tabRow.PlayerName.text = name;
        tabRow.CrownCount.text = crowns + "x";
    }
}