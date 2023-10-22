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

    // Update is called once per frame
    private void Start()
    {
        GameObject.Find("Local Player").GetComponent<LocalPlayer>()._inGameUI = this;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _game.SetActive(false);
            _menu.SetActive(true);
        }
    }

    public void ChangeCursorColor(Color color)
    {
        _cursor.GetComponent<Image>().color = color;
    }

    //menu control
    public void CloseMenu()
    {
        _game.SetActive(true);
        _menu.SetActive(false);
    }

    public void Disconect()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }

    public void DeathScreen(bool isAlive)
    {
        _deathScreen.SetActive(!isAlive);
        _cursor.SetActive(isAlive);
    }

    public void UpdateGameScreen(bool isOver)
    {
        _running.SetActive(!isOver);
        _over.SetActive(isOver);
    }
}
