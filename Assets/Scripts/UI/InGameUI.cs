using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _cursor;

    [SerializeField]
    private GameObject _menu;

    [SerializeField]
    private GameObject _hostJoinMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _cursor.SetActive(false);
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
        _cursor.SetActive(true);
        _menu.SetActive(false);
    }

    public void Disconect()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }
}
