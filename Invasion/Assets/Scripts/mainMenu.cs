using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class mainMenu : MonoBehaviour
{
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject audioPanel;
    //[SerializeField] GameObject self;


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

            EventSystem eventsystem = this.GetComponent<EventSystem>();

            if (eventsystem.enabled)
            {
                eventsystem.enabled = false;
            }

            else
            {
                eventsystem.enabled = true;
            }

        }
    }

   

    public void CreditsOpenAndClose()
    {
        if (audioPanel.activeSelf || controlsPanel.activeSelf)
        {
            return;
        } 
        
        
        else
        {
            creditsPanel.SetActive(!creditsPanel.activeSelf);

            EventSystem eventsystem = this.GetComponent<EventSystem>();

            if (eventsystem.enabled)
            {
                eventsystem.enabled = false;
            }

            else
            {
                eventsystem.enabled = true;
            }

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

            EventSystem eventsystem = this.GetComponent<EventSystem>();

            if (eventsystem.enabled)
            {
                eventsystem.enabled = false;
            }

            else
            {
                eventsystem.enabled = true;
            }
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
