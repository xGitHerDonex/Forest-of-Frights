using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkPoint : MonoBehaviour
{
    public bool isTriggered;
    [SerializeField] bool isFinalCheckpoint;
    [SerializeField] bool isMidBossCheckpoint;


    [SerializeField] GameObject barrierWall;
    [SerializeField] endBossAI endBossScript;
    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player") && gameManager.instance.player.transform.position != transform.position)
        {
            gameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(gameManager.instance.checkPointPopup());
            isTriggered = true;

            if (isFinalCheckpoint)
            {
                barrierWall.SetActive(true);
                endBossScript.finalCheckpointActivated = true;

            }

            else if (isMidBossCheckpoint)
            {
                if (barrierWall != null)
                    barrierWall.SetActive(true);


            }

        }


        
    }
}
