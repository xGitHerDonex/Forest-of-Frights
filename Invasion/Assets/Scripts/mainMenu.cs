using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject audioPanel;


    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }


    public void OnQuitButton()
    {
        Application.Quit();
    }


    public void ControlOpenClose()
    {
        if (audioPanel.activeSelf || creditsPanel.activeSelf)
        {
            return;
        } else
        {        
            controlsPanel.SetActive(!controlsPanel.activeSelf);

        }
    }

   

    public void CreditsOpenAndClose()
    {
        if (audioPanel.activeSelf || controlsPanel.activeSelf)
        {
            return;
        } else
        {
            creditsPanel.SetActive(!creditsPanel.activeSelf);

        }
    }


    public void AudioDisplayAndClose()
    {
        if (creditsPanel.activeSelf || controlsPanel.activeSelf)
        {
            return;
        } else
        {
        audioPanel.SetActive(!audioPanel.activeSelf);
        }
    }


    #region oldCode
    //public void closeCredits()
    //{
    //    if (audioPanel.activeSelf || controlsPanel.activeSelf)
    //    {
    //        return;
    //    } else
    //    {
    //        creditsPanel.SetActive(false);

    //    }
    //} 
    //public void closeControls()
    //{
    //    if (audioPanel.activeSelf || creditsPanel.activeSelf)
    //    {
    //        return;
    //    } else
    //    {
    //        controlsPanel.SetActive(false);

    //    }
    //} 
    #endregion
}
