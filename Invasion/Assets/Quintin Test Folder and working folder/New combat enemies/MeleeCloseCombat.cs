using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeCloseCombat : MonoBehaviour
{
    public BoxCollider AttackBox; // in this case the meleeBox collider must be child of the right foot
    public int meleeAttackDamage = 5;
    public AudioClip meleeAttackClip;

    BoxCollider m_meleeBox;

    void Awake()
    {
        AttackBox.isTrigger = true;

        m_meleeBox = GetComponent<BoxCollider>();
    }

    void MeleeAttack() // invoked by the kick animation events (in case you have many kick animations)
    {
       
        if (CheckBoxAndGiveDamage(m_meleeBox))
        {
            if (meleeAttackClip)
            {
                AudioSource.PlayClipAtPoint(meleeAttackClip, gameObject.transform.position); // play the sound
            }
        }
        
    }

    private void MeleeBoxOff()
    {
        m_meleeBox.enabled = false;
    }

    private void MeleeBoxOn()
    {
        m_meleeBox.enabled = true;
    }

    /// <summary>
    /// Returns true if the meleeBox collider overlap any collider with a health script. If overlap any collider also apply a the meleeAttackDamage.
    /// </summary>
    bool CheckBoxAndGiveDamage( BoxCollider meleeBox )
    {
        // check if we hit some object with a iDamage script added
        
        Collider[] colliders = Physics.OverlapBox(meleeBox.transform.position, meleeBox.size,meleeBox.transform.rotation,Physics.AllLayers, QueryTriggerInteraction.Ignore);
        foreach (Collider collider in colliders)
        {
            if (collider == m_meleeBox)
                continue; // ignore myself

            Collider contact = collider.GetComponent<Collider>();
            if (contact.gameObject.tag == gameManager.instance.player.tag)
            {
                // if the collider we overlap has a health, then apply the damage
               playerController pc = gameManager.instance.player.GetComponent<playerController>();
        pc.ApplyMeleeDamage(meleeAttackDamage);
                
                pc.physics(gameObject.transform.position.normalized);

                return true;
            }
        }

        return false;
    }

}
