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
    public GameObject Game;
    [SerializeField]
    private GameObject _menu;

    //Tab
    [SerializeField]
    private GameObject _tab;

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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _tab.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            _tab.SetActive(false);
        }
    }
    public void OnRoundStarted()
    {
        CurrentPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerObject.GetComponent<Player>();
        CurrentPlayer.GetComponent<LocalPlayer>().InGameUI = this;
    }

    //menu control
    public void CloseMenu()
    {
        SetMenu(false);
    }

    private void SetMenu(bool visible)
    {
        Game.SetActive(!visible);
        _menu.SetActive(visible);
        if (visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Disconect()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }

}
