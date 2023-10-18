using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class popupMessages : MonoBehaviour
{
    public Text BuffAlerts;
    private bool isInRange = false;
    // Start is called before the first frame update
    void Start()
    {
        BuffAlerts.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange)
        {
            BuffAlerts.gameObject.SetActive(true);
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
            BuffAlerts.gameObject.SetActive(false);
        }
    }
}
