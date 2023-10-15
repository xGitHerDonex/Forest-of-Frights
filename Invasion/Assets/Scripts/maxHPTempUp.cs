using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maxHPTempUp : MonoBehaviour
{
    [SerializeField] public float maxHPBuff;
    [SerializeField] public float buffDuration;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            gameManager.instance.playerScript.givemaxHPBuff(buffDuration, maxHPBuff);
            Destroy(gameObject);

        }
    }
}
