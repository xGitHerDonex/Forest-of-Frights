using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WpPatrol : MonoBehaviour, IPhysics//, IDamage
{

    /*The script can identify that the player’s character is in range, 
     * perform a Raycast and know whether anything has been hit.
     */
    //checks player transform instead of gameobject
    [SerializeField] Transform player;


    NavMeshAgent agent;
    [SerializeField] Transform[] waypoints;
    int m_PathIndex;
    public float animeSpeedChange;
    Animator anime;
    Vector3 direction;
    //for melee if player is in range
    bool m_IsPlayerAttackable;
    [SerializeField] float targetFaceSpeed = 1;

    private void Awake()
    {
        // player = gameManager.instance.player.transform;
        anime = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

    }

    // Start is called before the first frame update
    void Start()
    {
        direction = player.position - transform.position;
        StartPatrol();
    }

    // Update is called once per frame
    void Update()
    {
        float agentVel = agent.velocity.normalized.magnitude;
        anime.SetFloat("Speed", Mathf.Lerp(anime.GetFloat("Speed"), agentVel, Time.deltaTime * animeSpeedChange));
        IsAttackable();

        if (agent.isActiveAndEnabled)
        {
            if (!IsAttackable())
            {

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    m_PathIndex = (m_PathIndex + 1) % waypoints.Length;
                    agent.SetDestination(waypoints[m_PathIndex].position);
                }
            } else if (IsAttackable())
            {
                //vector math for direction is from target to destination Vector3 target-starting point
                //Vector3 direction = player.position - transform.position;
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
                        agent.isStopped = true;
                        agent.SetDestination(player.transform.position);
                        FaceTarget();
                        agent.isStopped = false;


                    }
                }
            }

        }
    }


    void OnTriggerEnter( Collider other )
    {
        if (other.transform == player.transform)
        {
            m_IsPlayerAttackable = true;
            anime.SetBool("isCaught", true);
            transform.position += Vector3.up;
            anime.SetBool("isLanding", false);
        }
    }
    private void OnTriggerExit( Collider other )
    {
        if (other.transform == player)
        {
            m_IsPlayerAttackable = false;
            anime.SetBool("isCaught", false);
            transform.position += Vector3.down;
            anime.SetBool("isLanding", true);
        }
    }



    void StartPatrol()
    {
        //just starts the enemy on the way to the initail waypoint.
        agent.SetDestination(waypoints[0].position.normalized);
        FaceTarget();

        //test code will integrate a default speed in enemy AI.
        //anime.SetFloat("isMoving", 1);

    }

    /// <summary>
    /// just returns if the player can be attacked or not 
    /// </summary>
    /// <returns></returns>
    public bool IsAttackable()
    {
        return m_IsPlayerAttackable;
    }

    public void physics( Vector3 dir )
    {
        agent.velocity += dir / 3;

    }

    void FaceTarget()
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        //lerp over time rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * targetFaceSpeed);
    }

}
