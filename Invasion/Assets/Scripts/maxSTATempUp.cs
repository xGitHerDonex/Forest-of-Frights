using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maxSTATempUp : MonoBehaviour
{
    [SerializeField] public float maxSTABuff;
    [SerializeField] public float buffDuration;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            gameManager.instance.playerScript.givemaxSTABuff(buffDuration, maxSTABuff);
            Destroy(gameObject);

        }
    }
}
