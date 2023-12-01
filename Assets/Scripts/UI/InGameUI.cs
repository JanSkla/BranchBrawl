using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _game;
    [SerializeField]
    private GameObject _menu;

    //Game
    [SerializeField]
    private GameObject _running;
    [SerializeField]
    private GameObject _over;

    //Running
    [SerializeField]
    private GameObject _cursor;
    [SerializeField]
    private GameObject _deathScreen;

    //Over
    [SerializeField]
    public TextMeshProUGUI PlacementText;

    public Player CurrentPlayer;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetMenu(!_menu.activeSelf);
        }
    }
    public void OnRoundStarted()
    {
        CurrentPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerObject.GetComponent<Player>();
        CurrentPlayer.GetComponent<LocalPlayer>().InGameUI = this;
    }

    public void ChangeCursorColor(Color color)
    {
        _cursor.GetComponent<Image>().color = color;
    }

    //menu control
    public void CloseMenu()
    {
        SetMenu(false);
    }

    private void SetMenu(bool visible)
    {
        if (_running.activeSelf)
        {
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }
        _game.SetActive(!visible);
        _menu.SetActive(visible);
    }

    public void Disconect()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }

    public void DeathScreen(bool isAlive)
    {
        Cursor.lockState = !isAlive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = !isAlive;
        _deathScreen.SetActive(!isAlive);
        _cursor.SetActive(isAlive);
    }

    public void UpdateGameScreen(bool isOver)
    {
        Cursor.lockState = isOver ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOver;
        _running.SetActive(!isOver);
        _over.SetActive(isOver);
    }
}
