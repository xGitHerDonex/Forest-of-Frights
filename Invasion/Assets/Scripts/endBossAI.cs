using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Device;
using UnityEngine.ProBuilder;

public class endBossAI : MonoBehaviour, IDamage, IPhysics
{
    [Header("-----Components-----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;
    [SerializeField] Animator anime;
    [SerializeField] Collider hitBox;
    [SerializeField] Rigidbody rb;

    [Header("-----Enemy Stats-----")]

    [Tooltip("Turning speed 1-10.")]
    [Range(1, 10)][SerializeField] int targetFaceSpeed;
    [SerializeField] int runningDistance;


    [Tooltip("Enemy health value between 1 and 100.")]
    [Range(1, 300)][SerializeField] int hp;
    [Range(1, 300)][SerializeField] int maxHp;


    [Tooltip("10 is the default value for all current speeds. Changing this without adjusting Enemy Speed and nav mesh speed will break it!!!!")]
    [Range(-30, 30)][SerializeField] float animeSpeedChange;
    [SerializeField] int enemyRunSpeed;


    [Header("-----Melee Components-----")]
    [SerializeField] GameObject leftMeleeCollider;
    [SerializeField] GameObject rightMeleeCollider;
    [Tooltip("Delay Value between melee attacks in seconds")]
    [SerializeField] int meleeDelay;
    [Tooltip("meleeDamage")]
    [SerializeField] int meleeDamage;
    [SerializeField] int meleeStage2Damage;


    [Header("-----FireBall Components-----")]
    [Tooltip("Object to Shoot")]
    [SerializeField] GameObject bullet;

    [Tooltip("Sets the ranged stopping distance (updates on range attacks)")]
    [SerializeField] float rangedStoppingDistance;

    [Tooltip("Max Distance enemy cans hoot energy balls")]
    [SerializeField] int maxShootingRange;

    [Tooltip("Used to delay the projectile instantiation to match animation")]
    [SerializeField] int rangeDelay;

    [Tooltip("Angle which the enemy can attack.")]
    [SerializeField] int viewAngle;
    [Range(-360, 360)][SerializeField] int shootAngle;

    [Tooltip("Rate enemy can attack")]
    [Range(0, 10)][SerializeField] float shootRate;
    [SerializeField] float shootDelay;

    [Header("-----Flight-----")]
    [SerializeField] GameObject waypoint;
    [SerializeField] int flightSpeed;
    [SerializeField] int origFlightSpeed;
    [SerializeField] int groundingHeight;
    [SerializeField] float distToWaypoint;


    [Header("SFX")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip walkSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip deathSound;

    Vector3 pushBack;
    Vector3 playerDirection;
    Vector3 startingPos;
    float stoppingDistOriginal;
    float speedOrig;
    float agentVel;
    float hpRatio;
    float distToPlayer;
    float angleToPlayer;
    bool isAttacking;
    bool isDead;
    bool playerInRange;
    bool isMoving;
    bool isRunning;
    bool isShooting;
    playerController playerScript;



 

    // Start is called before the first frame update
    void Start()
    {
        //Get's Player Script so we can check the closest waypoint
        playerScript = gameManager.instance.player.GetComponent<playerController>();
        origFlightSpeed = flightSpeed;
        isMoving = false;
    }

    

    // Update is called once per frame
    void Update()
    {
        //Continually checks to see if Enemy is flying, and updates animation
        setFlightAnimation();

        flyToTarget();
   
        

        
    }


    //Heads to specified Target
    IEnumerator headToTarget(GameObject target)
    {
        isMoving = true;
        faceTarget(target);
        Vector3 position = Vector3.MoveTowards(rb.position, target.transform.position, flightSpeed * Time.deltaTime);
        rb.MovePosition(position);

        yield return new WaitForSeconds(1);

        isMoving = false;
      
    }

    //Flys to Closest Ground Waypoint, when called
    void flyToTarget()
    {
        waypoint = playerScript.getClosestGroundWaypoint();
        distToWaypoint = Vector3.Distance(rb.position, waypoint.transform.position);

        if (distToWaypoint <= groundingHeight)
        {
            isMoving = false;
            flightSpeed = origFlightSpeed;
            rb.velocity = Vector3.zero;
        }

        else if (distToWaypoint >= groundingHeight)
        {
            flightSpeed = 30;
            StartCoroutine(headToTarget(waypoint));
        }
    }

    //Updates Flight animation 
    void setFlightAnimation()
    {

        if (rb.position.y > 0)
        {
            anime.SetBool("Fly", true);
        }

        else
        {
            anime.SetBool("Fly", false);
  
        }

               
    }

    //Faces selected Target
    void faceTarget(GameObject target)
    {
        //Get Targets _direction
        Vector3 targetDirection = target.transform.position - headPos.position;
        Quaternion rotation = Quaternion.LookRotation(targetDirection);

        //lerp over time rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * targetFaceSpeed);
    }


    //Faces Player
    void faceTarget()
    {
        Quaternion rotation = Quaternion.LookRotation(playerDirection);
        //lerp over time rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * targetFaceSpeed);
    }


    //Find Closest flight way point

    //Summon enemies

    //


    IEnumerator shoot()
    {
        isShooting = true;

        playAttackSound();
        anime.SetTrigger("Shoot");
        agent.ResetPath();

        //Used to add delay to the shoot to match the animation
        StartCoroutine(shootDelayed()); // DO NOT REMOVE - if you do not require a delay simply use 0 in the shootDelay variable

        yield return new WaitForSeconds(shootRate);
        isShooting = false;


    }

    //Update's speed parameter if target is running
    void updateRunningSpeed()
    {
        //If player is farther than the running distance set, then we update the animator to reflect the run speed and the enemy runs faster.
        if (distToPlayer >= runningDistance)
        {
            isRunning = true;
            agentVel = agent.velocity.normalized.magnitude + 1;
            agent.speed = enemyRunSpeed;
        }

        // If player is within the running -2 (to give the effect of them running up on the player), then we stop running and speed is set back to it's original
        else if (distToPlayer <= (runningDistance - 2))
        {
            isRunning = false;
            agentVel = agent.velocity.normalized.magnitude;
            agent.speed = speedOrig;
        }
    }

    //Allows the attached sound for Attack Sound to be played
    void playAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.clip = attackSound;
            audioSource.Play();
        }
    }
    //Melee Routine that will generate a number at random to determine attack sequence.
    IEnumerator Melee(int combo)
    {
        isAttacking = true;
        leftMeleeCollider.SetActive(true);
        rightMeleeCollider.SetActive(true);

        if (combo == 2)
            anime.SetTrigger("Melee2");

        if (combo == 3)
            anime.SetTrigger("Melee3");

        if (combo == 4)
            anime.SetTrigger("Melee4");


        yield return new WaitForSeconds(meleeDelay);
        isAttacking = false;
        leftMeleeCollider.SetActive(false);
        rightMeleeCollider.SetActive(false);
    }

    //Delay between Ranged attacks
    IEnumerator shootDelayed()
    {
        isAttacking = true;
        yield return new WaitForSeconds(shootDelay);
        isAttacking = false;
        Instantiate(bullet, shootPos.position, transform.rotation);

    }
    void playWalkSound()
    {
        if (audioSource != null && walkSound != null)
        {
            audioSource.clip = walkSound;
            audioSource.Play();
        }
    }

    //Runs damage on enemy, includes logic on death.
    public void takeDamage(int amount)
    {
        hp -= amount;
        StartCoroutine(stopMoving());


        if (hp <= 0)
        {
            isDead = true;
            hitBox.enabled = false;// turns off the hitbox so player isnt collided with the dead body
            agent.velocity = Vector3.zero;
            agent.enabled = false;
            anime.SetBool("Death", true);
            playDeathSound();

        }

        else
        {
            anime.SetTrigger("Damage");
            StartCoroutine(flashDamage());
            agent.SetDestination(gameManager.instance.player.transform.position);
        }



    }

    //Allows the attached sound for Death Sound to be played
    private void playDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.clip = deathSound;
            audioSource.Play();
        }
    }

    IEnumerator stopMoving()
    {
        agent.speed = 0;
        yield return new WaitForSeconds(.5f);
        agent.speed = speedOrig;
    }

    //changes the material color from the original material to a red color for .1 seconds
    //the changes the color back to its original white state.
    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    //Updates physics
    public void physics(Vector3 dir)
    {
        agent.velocity += dir / 3;

    }

    //this method is for starting a co-routine in case a delay is needed
    public void delayDamage(int damage, float seconds)
    {
        StartCoroutine(delayedDamage(damage, seconds));
    }

    //method made for triggering explosion damage through iPhysics
    public IEnumerator delayedDamage(int explosionDamage, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        takeDamage(explosionDamage);
    }


}
