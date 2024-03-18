using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.RayTracingAccelerationStructure;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _quitDialog;

    [SerializeField]
    private GameObject _sssettings;

    // SettingsBtn

    public void OpenSettings()
    {
        _sssettings.SetActive(true);
    }

    public void CloseSettings()
    {
        _sssettings.SetActive(false);
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
        Application.Quit();
    }
    
    public void SetResolution(int height)
    {
        int width = height / 9 * 16;
        Screen.SetResolution(width, height, false);
    }
}
