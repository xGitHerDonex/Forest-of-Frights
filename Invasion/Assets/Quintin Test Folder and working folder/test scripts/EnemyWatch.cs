using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWatch : MonoBehaviour
{
    /*The script can identify that the player’s character is in range, 
     * perform a Raycast and know whether anything has been hit.
     */
    //checks player transform instead of gameobject
    [SerializeField] Transform player;

    //for melee if player is in range
    bool m_IsPlayerAttackable;
    Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player.transform)
        {
            m_IsPlayerAttackable = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerAttackable = false;

        }
    }


    private void Update()
    {
        if (m_IsPlayerAttackable)
        {
            //vector math for direction is from target to destination Vector3 target-starting point
            Vector3 direction = player.position - transform.position;
            //this will be the line from the destination to check for a clear line of sight to target
            Ray ray = new Ray(transform.position, direction);
            //checking for colliders on ray to target with raycasthit
            RaycastHit raycastHit;

            #region debug

#if (UNITY_EDITOR)
            Debug.DrawRay(ray.origin, direction);
#endif 
            #endregion

            //Raycast method sets its data to information about whatever the Ray hit..
            //raycast is also giving out information as to what was hit in an out parameter to raycast hit
            if (Physics.Raycast(ray, out raycastHit))
            {
                //Next, it needs to check what has been hit.
                if (raycastHit.collider.transform == player)
                {
                    //if raycast has hit the player then this where the fun begins
                    //will use raycastHit information for targeting


                }
            }
        }
    }

}
