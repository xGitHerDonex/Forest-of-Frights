using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;
    [SerializeField] GameObject explosive;

    private void Start()
    {
        rb.velocity = gameManager.instance.player.transform.position  - transform.position.normalized * speed;
        StartCoroutine(explode());
    }

    public virtual IEnumerator explode()
    {
        yield return new WaitForSeconds(destroyTime);
        Instantiate(explosive, transform.position, explosive.transform.rotation);
        Destroy(gameObject);
    }
}
