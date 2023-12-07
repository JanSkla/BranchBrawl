using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private InGameUI _inGameUI;


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

    private void OnEnable()
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
        _cursor.SetActive(isAlive);
    }
}
