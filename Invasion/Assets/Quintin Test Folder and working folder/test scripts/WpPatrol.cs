using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class WpPatrol : MonoBehaviour, IPhysics, IDamage
{

    /*The script can identify that the player’s character is in range, 
     * perform a Raycast and know whether anything has been hit.
     */


    [Header("-----Components-----")]

    [Tooltip("checks player transform instead of gameobject")]
    [SerializeField] Transform player;
    [SerializeField] Collider hitBox;
    [SerializeField] Transform shootPos;
    [SerializeField] Renderer model;
    [Tooltip("Object to Shoot")]
    [SerializeField] GameObject bullet;
    [Tooltip("waypoints must be set")]
    [SerializeField] Transform[] waypoints;

    NavMeshAgent agent;
    Animator anime;

    
    
    

    [Header("-----Gun Stats and Bullet Component -----")]

    
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
    [Range(-360, 360)][SerializeField] int viewAngle;

    [Tooltip("Rate enemy can attack between 0 and 10.")]
    [Range(1, 10)][SerializeField] float shootRate;

    [Tooltip("1 is the default value for all current speeds. Changing this without adjusting Enemy Speed and nav mesh speed will break it!!!!")]
    [Range(-30, 30)][SerializeField] float animeSpeedChange = 1;


    [Header("SFX")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip walkSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip deathSound;
    
    
    bool isShooting;
    float speedOrig;
 //for melee if player is in range
    bool m_IsPlayerAttackable;
    int m_WpPointIndex;    
    Vector3 direction;
   


    private void Awake()
    {
        // player = gameManager.instance.player.transform;
        anime = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        StartPatrol();
    }

    // Start is called before the first frame update
    void Start()
    {
        direction = player.position - transform.position;
        speedOrig = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        //allows the enemy to ease into the transition animation with a tuneable
        //variable for custimization by Lerping it over time prevents choppy transitions
        float agentVel = agent.velocity.normalized.magnitude;
        anime.SetFloat("Speed", Mathf.Lerp(anime.GetFloat("Speed"), agentVel, Time.deltaTime * animeSpeedChange));

        IsAttackable();

        if (agent.isActiveAndEnabled)
        {
            if (!IsAttackable())
            {
                StartCoroutine(Roam());

            } else if (IsAttackable())
            {
                StartCoroutine(stopMoving());
                AttackSequence();
            }

        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player.transform)
        {
            m_IsPlayerAttackable = true;
            anime.SetBool("isCaught", true);
            transform.position += Vector3.up;
            anime.SetBool("isLanding", false);
        }
    }

    IEnumerator stopMoving()
    {
        agent.speed = 0;
        yield return new WaitForSeconds(1);
        agent.speed = speedOrig;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerAttackable = false;
            anime.SetBool("isCaught", false);
            transform.position += Vector3.down;
            anime.SetBool("isLanding", true);
        }
    }


    /// <summary>
    ///  starts the enemy on the way to roam
    /// </summary>
    void StartPatrol()
    {
        //just to the initail waypoint.
        //agent.SetDestination(waypoints[0].position.normalized);
        //test code will integrate a default speed in enemy AI.
        //anime.SetFloat("Speed", 1);
        StartCoroutine(Roam());

    }

    /// <summary>
    /// just returns if the player can be attacked or not 
    /// </summary>
    /// <returns></returns>
    public bool IsAttackable()
    {
        return m_IsPlayerAttackable;
    }

    public void physics(Vector3 dir)
    {
        agent.velocity += dir / 3;

    }

    /// <summary>
    /// lerps the turn speed not choppy (added but dont need)
    /// </summary>
    void FaceTarget()
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        //lerp over time rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * targetFaceSpeed);
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        StartCoroutine(stopMoving());


        if (hp <= 0)
        {
            hitBox.enabled = false; // turns off the hitbox so player isnt collided with the dead body
            agent.enabled = false;
            anime.SetBool("Death", true);
            playDeathSound();

            // Turn out the lights! (When the enemy dies)
            Light enemyLight = GetComponent<Light>();
            if (enemyLight != null)
            {
                enemyLight.enabled = false;
            }

            gameManager.instance.updateGameGoal(+1); //updates win condition set at 10 or greater "You win" also increments enemies killed and starts the win table Ienum

        } else
        {
            anime.SetTrigger("Damage");
            StartCoroutine(flashDamage());
            //agent.SetDestination(gameManager.instance.player.transform.position);
            AttackSequence();
        }

    }

    /// <summary>
    /// Allows the attached sound for Death Sound to be played
    /// </summary>
    private void playDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.clip = deathSound;
            audioSource.Play();
        }
    }

    //changes the material color from the original material to a red color for .1 seconds
    //the changes the color back to its original white state.
    /// <summary>
    /// flashes the model material
    /// </summary>
    /// <returns></returns>
    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }



    void AttackSequence()
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
                if (!isShooting && angleToPlayer <= shootAngle)
                {
                    StartCoroutine(shoot());
                }

            }
        }
    }

    // Set the shooting bool to true then
    // places and intializes the bullet object from the shooting postion and gives it a direction while triggering the bool that checks whether the enemy is shooting or not.
    //suspends the coroutine for the amount of seconds the shootrate is set to then sets the shooting back to false
    IEnumerator shoot()
    {
        isShooting = true;
        playAttackSound();
        anime.SetTrigger("Shoot");

        //Used to add delay to the shoot to match the animation
        StartCoroutine(shootDelayed()); // DO NOT REMOVE - if you do not require a delay simply use 0 in the shootDelay variable

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator shootDelayed()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootDelay);
    }
    //Allows the attached sound for Attack Sound to be played
    private void playAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.clip = attackSound;
            audioSource.Play();
        }
    }

    /// <summary>
    /// roams between a preset amount of waypoints
    /// </summary>
    /// <returns></returns>
    IEnumerator Roam()
    {
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            int randNum = Random.Range(0, 100);
            // m_WpPointIndex = (m_WpPointIndex + 1) % waypoints.Length;
            m_WpPointIndex = (randNum) % waypoints.Length;
            stopMoving();
            FaceTarget();
            agent.SetDestination(waypoints[m_WpPointIndex].position);
            yield return new WaitForSeconds(0.5f);
        }
    }
    ///this method is for starting a co-routine in case a delay is needed
    public void delayDamage(int damage, float seconds)
    {
        StartCoroutine(delayedDamage(damage, seconds));
    }

    ///method made for triggering explosion damage through iPhysics
    public IEnumerator delayedDamage(int explosionDamage, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        takeDamage(explosionDamage);
    }
}
