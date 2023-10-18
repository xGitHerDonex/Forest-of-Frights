using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showEnemy : MonoBehaviour
{
    [SerializeField] GameObject[] enemies; 
    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject enemy in enemies)
            {

                enemy.SetActive(true);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject enemy in enemies)
            {

                enemy.SetActive(false);

            }
        }
    }
    

}
