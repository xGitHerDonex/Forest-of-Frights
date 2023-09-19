using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Range(0, 50)][SerializeField] int explosionAmount;
    [Range(1, 5)][SerializeField] int explosionDamage;

    [SerializeField] GameObject explosionEffect;
    

    void Start()
    {
        Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation);
        Destroy(gameObject, 0.1f);

    }
    //Lecture Code 9-8-23 if Iphysics is attached to the other collider then multiply the explosion amount to the transm=form normalized between 0-1
    private void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger)
            return;

        IPhysics physicable = other.GetComponent<IPhysics>();
        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null)
            damageable.takeDamage(explosionDamage);

        if (physicable != null)
        {
            physicable.physics((other.transform.position - transform.position).normalized * explosionAmount);
           
        }


    }
}
