using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : NetworkBehaviour
{
    [SerializeField]
    private InGameUI _inGameUI;


    [SerializeField]
    private GameObject _preGame;
    [SerializeField]
    private GameObject _running;
    [SerializeField]
    private GameObject _over;


    //Running
    [SerializeField]
    private GameObject _cursor;
    [SerializeField]
    private GameObject _deathScreen;
    [SerializeField]
    private GameObject _deathBackdrop;
    [SerializeField]
    public HealthDisplayText HealthDiplay;

    //Over
    [SerializeField]
    public TextMeshProUGUI PlacementText;

    [SerializeField]
    public NetworkCountdownText CountDownText;

    private GameManager _gameManager;

    void Start()
    {
        _preGame.SetActive(true);
        _running.SetActive(true);
        _over.SetActive(false);
    }
    public void OnMenuClose()
    {
        if (_running.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void UpdateGameScreen(bool isOver)
    {
        Cursor.lockState = isOver ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOver;
        _preGame.SetActive(false);
        _running.SetActive(!isOver);
        _over.SetActive(isOver);
    }

    public void ChangeCursorColor(Color color)
    {
        _cursor.GetComponent<Image>().color = color;
    }

    public void DeathScreen(bool isAlive)
    {
        Cursor.lockState = !isAlive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = !isAlive;
        _deathScreen.SetActive(!isAlive);
        _deathBackdrop.SetActive(!isAlive);
        _cursor.SetActive(isAlive);
    }
}
