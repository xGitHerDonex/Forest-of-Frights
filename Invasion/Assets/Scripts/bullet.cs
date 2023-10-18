using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] float destroyTime = 5f;



    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
        Destruction();
       

    }
    

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("walls"))
        {
            Destroy(gameObject);
        } else
        {

            if (other.isTrigger)
                return;


            IDamage damageable = other.GetComponent<IDamage>();

            if (damageable != null)
            {
                damageable.hurtBaddies(damage);
            }
            Destruction();
        }
    }

    void Destruction()
    {
        if(destroyTime > 0)
        {
            Destroy(gameObject, destroyTime);
        }
    }
}
