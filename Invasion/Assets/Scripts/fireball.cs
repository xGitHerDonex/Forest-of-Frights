using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class fireball : MonoBehaviour 
{

    [SerializeField] Rigidbody rb;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] int explosionDamage;
    [Range(0, 50)][SerializeField] int explosionAmount;

    void Start()
    {
        rb.velocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
        StartCoroutine(explode());
    }

    IEnumerator explode()
    {
        yield return new WaitForSeconds(destroyTime);
        BombsAway();
    }



   void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        BombsAway();
    }

    public void BombsAway()
    {
        Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation);
        Destroy(gameObject);
    }
}






