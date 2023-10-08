using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;

public class explosion : MonoBehaviour
{
    [Range(0, 50)][SerializeField] int explosionAmount;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] int explosionDamage;
    [SerializeField] float destroyTime; // do not initialize variables in the class -do so in unity

    protected void Start()
    {
        BombsAway();

    }
    //Lecture Code 9-8-23 if Iphysics is attached to the other collider then multiply the explosion amount to the transm=form normalized between 0-1
   void OnTriggerEnter(Collider other)
    {

        ProcessDamage(other);

    }

 public void BombsAway()
        {
            Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation);
            Destroy(gameObject, destroyTime);
        }

    public void ProcessDamage(Collider other )
    {
        if (other.isTrigger)
            return;

        IPhysics physicable = other.GetComponent<IPhysics>();
        IDamage damageable = other.GetComponent<IDamage>();

        if (physicable != null)
        {
            physicable.physics((other.transform.position - transform.position).normalized * explosionAmount);
            damageable.delayDamage(explosionDamage, 0.2f);
        }
    }

}
