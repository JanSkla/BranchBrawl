using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _quitDialog;

    // PlayBtn

    public void PlayGame()
    {
        SceneManager.LoadScene("HostJoinMenu");
    }

    // SettingsBtn

    public void OpenSettings()
    {
        Debug.Log("settings");
    }

    // QuitBtn

    public void OpenQuitDialog()
    {
        _quitDialog.SetActive(true);
    }

    public void CancelQuit()
    {
        _quitDialog.SetActive(false);
    }

    public void Quit()
    {
        Quit();
    }
}
