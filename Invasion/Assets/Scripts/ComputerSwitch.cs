using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComputerSwitch : MonoBehaviour
{
    public TextMeshProUGUI activateMessage;
    public Material greenMaterial;
    public GameObject door;
    public GameObject popupAlert;
    public GameObject monitor;
    public AudioClip activateSound;

    private bool isInRange = false;
    private bool isSwitchActivated = false;
    private AudioSource audioSource;

    void Start()
    {
        activateMessage.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isInRange && !isSwitchActivated)
        {
            activateMessage.gameObject.SetActive(true);

            if (Input.GetButtonDown("Activate"))
            {
                Renderer monitorRender = monitor.GetComponent<Renderer>();
                if (monitorRender != null)
                {
                    monitorRender.material = greenMaterial;
                    Destroy(door);
                    Destroy(popupAlert);
                    isSwitchActivated = true;
                    activateMessage.gameObject.SetActive(false);
                    audioSource.PlayOneShot(activateSound);
                }
            }

            


        }
        //else
        //{
        //    activateMessage.gameObject.SetActive(false);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            activateMessage.gameObject.SetActive(false);
        }
    }
}
