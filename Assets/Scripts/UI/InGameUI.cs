using System.Collections;
using System.Collections.Generic;
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

    //Runngig
    [SerializeField]
    private GameObject _cursor;
    [SerializeField]
    private GameObject _deathScreen;

    public Player CurrentPlayer;

    void Start()
    {
        Cursor.visible = false;
        CurrentPlayer = GameObject.Find("Local Player").GetComponent<Player>();
        CurrentPlayer.GetComponent<LocalPlayer>().InGameUI = this;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetMenu(!_menu.activeSelf);
        }
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
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Confined; ///HERE
        //Cursor.visible = visible;
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
        Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        _deathScreen.SetActive(!isAlive);
        _cursor.SetActive(isAlive);
    }

    public void UpdateGameScreen(bool isOver)
    {
        Debug.Log("Over");
        _running.SetActive(!isOver);
        _over.SetActive(isOver);
    }
}
