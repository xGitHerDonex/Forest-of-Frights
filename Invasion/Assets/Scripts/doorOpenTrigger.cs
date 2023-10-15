using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorOpenTrigger : MonoBehaviour
{
    public bool isSwitchActivated = false;
    public AudioSource audioSource;
    public AudioClip activateSound;

    public void ActivateSwitch()
    {
        if (isSwitchActivated)
        {
            audioSource.PlayOneShot(activateSound);
            Destroy(gameObject);         
        }
    }
}
