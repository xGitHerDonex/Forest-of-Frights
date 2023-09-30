using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;



    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
        Destroy(gameObject, destroyTime);

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger)
            return;


        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null)
        {
            damageable.takeDamage(damage);
        }
        Destroy(gameObject);
    }
}
