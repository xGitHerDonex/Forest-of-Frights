using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class fireball : explosion
{

    [SerializeField] Rigidbody rb;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;
    [SerializeField] GameObject explosive;

    protected override void Start()
    {
        rb.velocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
        StartCoroutine(explode());
    }

    public virtual IEnumerator explode()
    {
        yield return new WaitForSeconds(destroyTime);
        BombsAway();
        Destroy(gameObject);
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        BombsAway();
    }
}






