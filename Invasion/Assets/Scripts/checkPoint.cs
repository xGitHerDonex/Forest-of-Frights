using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkPoint : MonoBehaviour
{
    bool isTriggered;
    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player") && gameManager.instance.player.transform.position != transform.position)
        {
            gameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(gameManager.instance.checkPointPopup());
            isTriggered = true;
        }


        
    }
}
