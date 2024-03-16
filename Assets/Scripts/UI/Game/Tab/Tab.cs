using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Tab : MonoBehaviour
{
    [SerializeField]
    private GameObject _tabContainer;
    [SerializeField]
    private GameObject _tabRowPrefab;

    private GameManager _gameManager;


    void OnEnable()
    {
        Debug.Log("sda");
        _gameManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();
        foreach (Transform child in _tabContainer.transform)
        {
            Destroy(child.gameObject);
        }

        List<PlayerGameData> sortedList = new();

        Debug.Log("sda");
        foreach (var data in _gameManager.PlayersGameData)
        {
            sortedList.Add(data);
        }

        sortedList = sortedList.OrderBy(o => o.Crowns).Reverse().ToList();
        Debug.Log("sda");
        for (int i = 0; i < sortedList.Count; i++)
        {
            var data = sortedList[i];;
            AddRow(data.PlayerName.ToString(), data.Crowns);
        }
        Debug.Log("sda");
    }

    private void AddRow(string name, int crowns)
    {
        var tabRow = Instantiate(_tabRowPrefab, _tabContainer.transform).GetComponent<TabRow>();
        tabRow.PlayerName.text = name;
        tabRow.CrownCount.text = crowns + "x";
    }
}
