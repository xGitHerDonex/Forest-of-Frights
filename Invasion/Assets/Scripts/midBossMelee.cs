using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class midBossMelee : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] midBossAI bossMelee;

    private void Start()
    {
        damage = bossMelee.meleeDamage;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger)
            return;

        //Damages object during collision
        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null)
        {
            damage = bossMelee.meleeDamage;
            damageable.hurtBaddies(damage);
        }

    }
}
