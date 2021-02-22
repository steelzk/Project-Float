using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steel.Networking;
using Doozy.Engine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private UIView pauseView;
    [SerializeField] private UIView settingsView;
    private SPlayerController playerController;

    private void OnEnable()
    {
        playerController = transform.root.GetComponent<SPlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseView.IsVisible)
            {
                HidePause();
            } else
            {
                ShowPause();
            }
        }
    }

    public void ShowPause()
    {
        playerController.LockCursor(false);
        pauseView.Show();
    }

    public void HidePause()
    {
        playerController.LockCursor();
        pauseView.Hide();
    }

    public void OpenSettings()
    {
        settingsView.Show();
    }

    public void CloseSettings()
    {
        settingsView.Hide();
    }

    public void Disconnect()
    {
        SNetworkManager.singleton.StopClient();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
