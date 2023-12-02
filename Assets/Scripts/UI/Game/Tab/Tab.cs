using System.Collections;
using System.Collections.Generic;
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

        foreach (var data in _gameManager.PlayersGameData)
        {
            AddRow(data.ClientId.ToString(), data.Crowns);
        }
    }

    private void AddRow(string name, int crowns)
    {
        var tabRow = Instantiate(_tabRowPrefab, _tabContainer.transform).GetComponent<TabRow>();
        tabRow.PlayerName.text = name;
        tabRow.CrownCount.text = crowns + "x";
    }
}
