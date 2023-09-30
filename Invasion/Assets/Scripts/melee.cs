using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class melee : MonoBehaviour
{
    [SerializeField] int damage;


    private void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger)
            return;

        //Damages object during collision
        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null)
        {
            damageable.takeDamage(damage);
        }

    }
}
