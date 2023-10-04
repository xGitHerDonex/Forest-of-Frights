using System;
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
    #region Variables
    #region Auto Set Components

    NavMeshAgent agent;
    Animator anime;
    #endregion

    [Header("-----Components-----")]

    [Tooltip("checks player transform instead of gameobject")]
    [SerializeField] Transform player;

    [Tooltip("Object to Shoot. This one takes a CAPSULE COLLIDER")]
    [SerializeField] CapsuleCollider hitBox;

    [Tooltip("Position being shot from")]
    [SerializeField] Transform shootPos;

    [Tooltip("Model to display damage")]
    [SerializeField] Renderer model;

    [Tooltip("Object being Shot")]
    [SerializeField] GameObject bullet;
    [Tooltip("Object being Shot")]
    [SerializeField] ParticleSystem m_RangedAttackParticle;

    [Tooltip("waypoints must be set")]
    [SerializeField] Transform[] waypoints;



    [Header("-----Gun Stats  -----")]

    [Tooltip("Used to delaying the projectile instantiation to match animation")]
    [SerializeField] float shootDelay;


    [Header("-----Enemy Stats-----")]
    [Tooltip("Turning speed 1-10.")]
    [Range(1, 10)][SerializeField] float targetFaceSpeed = 1;

    [Tooltip("life points")]
    [SerializeField] int hp;

    [Tooltip("Angle which the enemy can attack. (-)360-360")]
    [Range(-360, 360)][SerializeField] int shootAngle;

    [Tooltip("Enemy viewing angle, (-)360-360.")]
    [Range(-360, 360)][SerializeField] int m_ViewAngle;

    [Tooltip("Rate enemy can attack between 0 and 10.")]
    [Range(1, 10)][SerializeField] float shootRate;

    [Tooltip("1 is the default value for all current speeds. Changing this without adjusting Enemy Speed and nav mesh speed will break it!!!!")]
    [Range(-30, 30)][SerializeField] float animeSpeedChange = 1;



    [Header("SFX")]
    [SerializeField] AudioSource m_AudioSource;
    [SerializeField] AudioClip m_WalkSound;
    [SerializeField] AudioClip m_AttackSound;
    [SerializeField] AudioClip m_DeathSound;

    //for melee if player is in range
    bool m_IsPlayerAttackable;
    int m_PathIndex;
    private Vector3 direction;
    bool m_IsShooting;
    float m_SpeedOrig;
    float m_AngleToPlayer;

    #endregion
    private void Awake()
    {
        // player = gameManager.instance.player.transform;
        anime = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
       // hitBox = gameObject.GetComponent<CapsuleCollider>();
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
                    if (raycastHit.collider.transform == player && m_AngleToPlayer <= m_ViewAngle)
                    {

                        //if raycast has hit the player then this where the fun begins
                        //will use raycastHit information for targeting
                        agent.isStopped = true;
                        agent.SetDestination(player.transform.position);
                        FaceTarget();
                        agent.isStopped = false;
                        SphereCollider sphereCollider = gameObject.GetComponent<SphereCollider>();
                        if (sphereCollider.bounds.Contains(player.transform.position))
                        {
                            if (!m_IsShooting && m_AngleToPlayer <= shootAngle)
                            {
                                StartCoroutine(shoot());
                            }
                            
                        }

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
            Fly();
        }
    }

    private void Fly()
    {
        anime.SetBool("isCaught", true);
        transform.position += Vector3.up;
            anime.SetBool("isLanding", false);
        
    }

    private void OnTriggerExit( Collider other )
    {
        if (other.transform == player)
        {
            m_IsPlayerAttackable = false;
            Land();

        }
    }

    private void Land()
    {
        anime.SetBool("isCaught", false);
        transform.position += Vector3.down;
        anime.SetBool("isLanding", true);
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

    IEnumerator shoot()
    {
        m_IsShooting = true;
        //playAttackSound();
        anime.SetTrigger("Shoot");

        //Used to add delay to the shoot to match the animation
        StartCoroutine(shootDelayed()); // DO NOT REMOVE - if you do not require a delay simply use 0 in the shootDelay variable


        yield return new WaitForSeconds(shootRate);
        m_IsShooting = false;
    }

    IEnumerator shootDelayed()
    {
        Instantiate(bullet, shootPos.position, Quaternion.identity);
        yield return new WaitForSeconds(shootDelay);
    }
}
