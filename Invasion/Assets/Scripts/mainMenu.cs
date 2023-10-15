using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject creditsPanel;

    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }


    public void OnQuitButton()
    {
        Application.Quit();
    }


    public void showControls()
    {
        controlsPanel.SetActive(true);
    }

    public void closeControls()
    {
        controlsPanel.SetActive(false);  
    }

    public void showCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void closeCredits()
    {
        creditsPanel.SetActive(false);
    }
}
