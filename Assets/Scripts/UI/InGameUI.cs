using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public bool HideCursorWhenExitingMenu;
    public bool AllowControlsWhenExitingMenu;

    [SerializeField]
    public GameObject Game;
    [SerializeField]
    private GameObject _menu;

    //Tab
    [SerializeField]
    private GameObject _tab;

    [SerializeField]
    public GameObject FireEffectScreen;


    private Player _currentPlayer;
    public Player CurrentPlayer
    {
        get { return _currentPlayer; }
        set
        {
            _currentPlayer = value;
            OnCurrentPlayerSet();
        }
    }
    private void OnCurrentPlayerSet()
    {
        if (CurrentPlayer.IsUnityNull()) return;
        var gameUI = Game.GetComponent<GameUI>();
        if (gameUI.IsUnityNull()) return;
        gameUI.HealthDisplay.ConnectHealthToPlayer(CurrentPlayer);
    }
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_menu.activeSelf)
            {
                SetMenu(false);
            }
            else
            {
                SetMenu(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _tab.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            _tab.SetActive(false);
        }
    }
    //public void OnRoundStarted()
    //{
    //    //CurrentPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerObject.GetComponent<Player>();
    //    //CurrentPlayer.GetComponent<LocalPlayer>().InGameUI = this;
    //}

    //menu control
    public void CloseMenu()
    {
        SetMenu(false);
    }

    private void SetMenu(bool visible)
    {
        //Game.SetActive(!visible);
        _menu.SetActive(visible);
        if (visible)
        {
                CurrentPlayer.AreControlsDisabled = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
        }
        else
        {
            if (AllowControlsWhenExitingMenu)
                CurrentPlayer.AreControlsDisabled = false;
            if (HideCursorWhenExitingMenu)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void Disconect()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }

}
