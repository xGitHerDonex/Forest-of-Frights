using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class popupMessages : MonoBehaviour
{
    public Text BuffAlerts;
    public Text ShootAlert;
    public Text GrenadeAlert;
    private bool isInRange = false;
    // Start is called before the first frame update
    void Start()
    {
       
        if (BuffAlerts != null)
        {
            BuffAlerts.gameObject.SetActive(false);
        }
      
        if (ShootAlert != null)
        {
            ShootAlert.gameObject.SetActive(false);
        }

        if (GrenadeAlert != null)
        {
            GrenadeAlert.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isInRange)
        {
           
            if (BuffAlerts != null && !string.IsNullOrEmpty(BuffAlerts.text))
            {
                BuffAlerts.gameObject.SetActive(true);
            }

           
            if (ShootAlert != null && !string.IsNullOrEmpty(ShootAlert.text))
            {
                ShootAlert.gameObject.SetActive(true);
            }

            if (GrenadeAlert != null && !string.IsNullOrEmpty(GrenadeAlert.text))
            {
                GrenadeAlert.gameObject.SetActive(true);
            }
        }
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

            // Deactivate BuffAlerts if it's assigned
            if (BuffAlerts != null)
            {
                BuffAlerts.gameObject.SetActive(false);
            }

            // Deactivate ShootAlert if it's assigned
            if (ShootAlert != null)
            {
                ShootAlert.gameObject.SetActive(false);
            }

            if (GrenadeAlert != null)
            {
                GrenadeAlert.gameObject.SetActive(false);
            }
        }
    }
}
