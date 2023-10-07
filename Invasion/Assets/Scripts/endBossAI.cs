using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Device;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class endBossAI : MonoBehaviour, IDamage, IPhysics
{
    [Header("-----Components-----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Transform bodyPos;
    [SerializeField] Transform shootPos;
    [SerializeField] Animator anime;
    [SerializeField] Collider hitBox;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject DemonLord;

    [Header("-----Enemy Stats-----")]

    [Tooltip("Turning speed 1-10.")]
    [Range(1, 20)][SerializeField] int targetFaceSpeed;
    [Range(1, 20)][SerializeField] int originalTargetFaceSpeed;
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

    [Tooltip("Calculates within stopping range where enemy will whip")]
    [SerializeField] float whipDelay;
    [Tooltip("Delay Value between melee attacks in seconds")]
    [SerializeField] float meleeDelay;
    [Tooltip("meleeDamage")]
    [SerializeField] int meleeDamage;
    [SerializeField] int meleeStage2Damage;
    [SerializeField] float meleeRange;


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
    [SerializeField] int flightSpeed;
    [SerializeField] int origFlightSpeed;
    [SerializeField] int groundingHeight;
    [SerializeField] int flightStoppingDist;
    [SerializeField] int landingSpeed;
    [SerializeField] int switchFacing;
    [SerializeField] float landingDelay;
    [SerializeField] int flightTargetFaceSpeed;

    [Header("-----Bools-----")]
    [SerializeField] bool startFight;
    [SerializeField] bool isLanding;
    [SerializeField] bool isAttacking;
    [SerializeField] bool isRunning;
    [SerializeField] bool isShooting;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isFlying;
    [SerializeField] bool reachedTarget;
    [SerializeField] bool isSummoning;
    [SerializeField] bool summonCompleted;
    [SerializeField] bool isRbDestroyed;



    [Header("-----Waypoints-----")]
    [SerializeField] float distToWaypoint;
    [SerializeField] float closestWaypointDist;
    [SerializeField] GameObject waypoint;
    [SerializeField] GameObject[] waypoints;
    [SerializeField] GameObject closestWaypoint;
    [SerializeField] float waypointDist;

    [Header("-----Summon Details-----")]
    [SerializeField] GameObject bossSpawner;
    [SerializeField] int summonTime;



    [Header("SFX")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip walkSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip deathSound;


    //player or enemy
    float agentVel;
    float hpRatio;
    float distToPlayer;
    float angleToPlayer;
    bool isDead;
    bool playerInRange;
    playerController playerScript;
    float stoppingDistOriginal;
    Vector3 pushBack;
    Vector3 playerDirection;
    Vector3 startingPos;
    float speedOrig;







    // Start is called before the first frame update
    void Start()
    {
        //Get's Player Script so we can check the closest waypoint

        playerScript = gameManager.instance.player.GetComponent<playerController>();
        origFlightSpeed = flightSpeed;
        stoppingDistOriginal = agent.stoppingDistance;
        isDead = false;
        isAttacking = false;
        isGrounded = true;
        isFlying = false;
        reachedTarget = false;
        summonCompleted = true;
        originalTargetFaceSpeed = targetFaceSpeed;

        //Creates Flight waypoint Matrix
        waypoints = GameObject.FindGameObjectsWithTag("FLWP");
    }



    // Update is called once per frame
    void Update()
    {

        if (startFight)//gameManager.instance.getIsMidBossDead())
        {

            //Continually checks to see if Enemy is flying, and updates animation
            setFlightAnimation();

            //Check HP levels
            float hpRatio = (float)(hp / maxHp);

            if (isSummoning)
            {
                summonRoutine(findClosestFlightWaypoint());
            }

            //Selects stage for enemy AI based on Health Remaining
            else if (hpRatio >= 0.8 && !isSummoning)
            {
                Stage1();
            }

            else if (hpRatio <= 0.8 && !isSummoning)
            {

                if (isFlying && summonCompleted && !isGrounded)
                {
                    flyToGround(playerScript.getClosestGroundWaypoint());

                }

                else if (!isTimeToSummon() && isGrounded && !isLanding && agent.isActiveAndEnabled)
                {

                    Stage2();

                }
            }

        }

    }

    void Stage1()
    {
        //If enemy is not dead, we'll continue
        if (!isDead)
        {
            facePlayer();
            //casts a ray on the player
            RaycastHit hit;

            targetFaceSpeed = originalTargetFaceSpeed;

            //get's player's direction
            playerDirection = gameManager.instance.player.transform.position - headPos.position;


            //Casts a ray on the player
            if (Physics.Raycast(headPos.position, playerDirection, out hit))
            {

                agentVel = agent.velocity.normalized.magnitude;
                anime.SetFloat("Speed", Mathf.Lerp(anime.GetFloat("Speed"), agentVel, Time.deltaTime * animeSpeedChange));

                //Gets distance to player
                distToPlayer = Vector3.Distance(headPos.position, gameManager.instance.player.transform.position);


                //If player within stopping distance, face target and attack if not already attacking
                if (!isAttacking && !isShooting && hit.collider.CompareTag("Player") && distToPlayer <= agent.stoppingDistance + 5 && distToPlayer >= meleeRange)
                {

                    agent.velocity = Vector3.zero;
                    agent.ResetPath();
                    facePlayer();
                    StartCoroutine(whip());

                }

                //If player within stopping distance, face target and attack if not already attacking
                else if (!isAttacking && !isShooting && hit.collider.CompareTag("Player") && distToPlayer <= meleeRange)
                {

                    agent.velocity = Vector3.zero;
                    agent.ResetPath();
                    facePlayer();
                    StartCoroutine(Melee(Random.Range(1, 5)));

                }

                //if player is not wthin stopping distance, then set distination to the player
                else if (hit.collider.CompareTag("Player") && distToPlayer >= agent.stoppingDistance)
                {

                    facePlayer();
                    agent.SetDestination(gameManager.instance.player.transform.position);

                }
            }

        }
    }

    void Stage2()
    {
        //If enemy is not dead, we'll continue
        if (!isDead)
        {
            facePlayer();
            //casts a ray on the player
            RaycastHit hit;

            targetFaceSpeed = originalTargetFaceSpeed;

            //get's player's direction
            playerDirection = gameManager.instance.player.transform.position - headPos.position;


            //Casts a ray on the player
            if (Physics.Raycast(headPos.position, playerDirection, out hit))
            {

                agentVel = agent.velocity.normalized.magnitude;
                anime.SetFloat("Speed", Mathf.Lerp(anime.GetFloat("Speed"), agentVel, Time.deltaTime * animeSpeedChange));

                //Gets distance to player
                distToPlayer = Vector3.Distance(headPos.position, gameManager.instance.player.transform.position);


                //If player within stopping distance, face target and attack if not already attacking
                if (!isAttacking && !isShooting && hit.collider.CompareTag("Player") && distToPlayer <= agent.stoppingDistance + 5 && distToPlayer >= meleeRange)
                {

                    agent.velocity = Vector3.zero;
                    agent.ResetPath();
                    facePlayer();
                    StartCoroutine(whip());

                }

                //If player within stopping distance, face target and attack if not already attacking
                else if (!isAttacking && !isShooting && hit.collider.CompareTag("Player") && distToPlayer <= meleeRange)
                {

                    agent.velocity = Vector3.zero;
                    agent.ResetPath();
                    facePlayer();
                    StartCoroutine(Melee(Random.Range(1, 5)));

                }

                else if (!isAttacking && !isShooting && hit.collider.CompareTag("Player") && distToPlayer >= agent.stoppingDistance && distToPlayer <= maxShootingRange)
                {


                    agent.velocity = Vector3.zero;
                    agent.ResetPath();
                    facePlayer();
                    StartCoroutine(shoot());
                    agent.SetDestination(gameManager.instance.player.transform.position);

                }


                //if player is not wthin stopping distance, then set distination to the player
                else if (hit.collider.CompareTag("Player") && distToPlayer >= agent.stoppingDistance)
                {

                    facePlayer();
                    agent.SetDestination(gameManager.instance.player.transform.position);

                }
            }

        }
    }

    bool isTimeToSummon()
    {
        if (Random.Range(0, 6000) <= 3)
        {
            isSummoning = true;
            return true;
        }

        else
            return false;
    }


    void summonRoutine(GameObject waypoint)
    {
        if(!reachedTarget)
            flyToTarget(waypoint);

        else if (reachedTarget && isFlying)
        {
            anime.SetTrigger("summon");
            summonCompleted = false;
            StartCoroutine(summon());
            reachedTarget = false;
        }
         

    }


    //Heads to specified Target
    IEnumerator headToTarget(GameObject target, bool flyToGround, bool flight = true)
    {
        float distToTarget = Vector3.Distance(rb.position, target.transform.position);

        float switchViewTarget = groundingHeight + switchFacing;

        if (flyToGround && distToTarget >= switchViewTarget)
        {
            faceTarget(target);
        }
        
        else if (flyToGround && distToTarget <= switchViewTarget )
        {
            facePlayer();
        }
    
        Vector3 position;

        if (flight)
        {
           position = Vector3.MoveTowards(rb.position, target.transform.position, flightSpeed * Time.deltaTime);
        }

        else
        {
            position = Vector3.MoveTowards(rb.position, target.transform.position, agent.speed * Time.deltaTime);
        }

       

        rb.Move(position,transform.rotation);

        yield return new WaitForSeconds(1);


    }

    //Flys to Closest Ground Waypoint, when called
    void flyToGround (GameObject waypoint)
    {

            isGrounded = false;
            isFlying = true;
            agent.enabled = false;

            distToWaypoint = Vector3.Distance(rb.position, waypoint.transform.position);

            if (distToWaypoint <= groundingHeight)
            {
                isFlying = false;
                reachedTarget = true;
                isLanding = true;
                StartCoroutine(land());


             }

            else if (distToWaypoint >= groundingHeight)
            {
                isFlying = true;
                StartCoroutine(headToTarget(waypoint,true));
            }

           
        
    }

    void createRb()
    {
        if (isRbDestroyed)
        {
            isRbDestroyed = false;
            rb = DemonLord.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.mass = 30;
        }
    }

    //Same as above, but enemy flys to any target
    void flyToTarget(GameObject waypoint)
    {

            createRb();

            isFlying = true;
            //rb.useGravity = false;
            isGrounded = false;
            agent.enabled = false;
            targetFaceSpeed = flightTargetFaceSpeed;
           
            distToWaypoint = Vector3.Distance(rb.position, waypoint.transform.position);


            if (distToWaypoint <= flightStoppingDist)
            {                
                reachedTarget = true;

            }

            else if (distToWaypoint >= flightStoppingDist)
            {
                faceTarget(waypoint);
                StartCoroutine(headToTarget(waypoint, false, true));
            }
        
    }

    //Land Routine that will land the enemy and reenable nav mesh
    IEnumerator land()
    {
        
        if (isLanding)
        {

            if (!isGrounded && !isFlying)
            {
                facePlayer();
                flightSpeed = origFlightSpeed;                            
                rb.velocity = new Vector3(0, landingSpeed, 0);
                rb.useGravity = true;
                setFlightAnimation();

                yield return new WaitForSeconds(landingDelay);
                isLanding = false;
                anime.SetTrigger("equip");
                agent.enabled = true;              
                isGrounded = true;
                reachedTarget = false;
                Destroy(rb);
                isRbDestroyed = true;



            }

        }

 
           
    }

    GameObject findClosestFlightWaypoint()
    {

        closestWaypointDist = 3000f;

        //iterates through the list to find the distances
        foreach (GameObject waypoint in waypoints)
        {
            //calculates distance
            waypointDist = Vector3.Distance(transform.position, waypoint.transform.position);


            //updates closest waypoint
            if (waypointDist < closestWaypointDist)
            {
                closestWaypointDist = waypointDist;
                closestWaypoint = waypoint;
            }

        }

        //returns the waypoint
        return closestWaypoint;
    }

    GameObject PickFlightWaypoint(float distMin, float distMax)
    {
        closestWaypointDist = 3000f;

        //iterates through the list to find the distances
        foreach (GameObject waypoint in waypoints)
        {
            //calculates distance
            waypointDist = Vector3.Distance(transform.position, waypoint.transform.position);

            if (waypointDist <= distMax && waypointDist >= distMin)
            {
                closestWaypoint = waypoint;
               
            }

        }

        return closestWaypoint;

    }

    //Updates Flight animation 
    void setFlightAnimation()
    {

        if (isFlying)
        {
            anime.SetBool("Fly", true);

        }

        else if (!isFlying)
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
    void facePlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(playerDirection);
        //lerp over time rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * targetFaceSpeed);
    }

    //Summon enemies using the spawn manager
    IEnumerator summon()
    {
        isSummoning = false;
        bossSpawnerManager.instance.setTimeToSpawn(true);
        yield return new WaitForSeconds(summonTime);
        bossSpawnerManager.instance.setTimeToSpawn(false);
        summonCompleted = true;



    }


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

            switch (combo)
            {
                case 1:
                    anime.SetTrigger("Melee1");
                    break;
                case 2:
                    anime.SetTrigger("Melee2");
                    break;
                case 3:
                    anime.SetTrigger("Melee3");
                    break;
                case 4:
                    anime.SetTrigger("Melee4");
                    break;

            }
   
     
        yield return new WaitForSeconds(meleeDelay);
        isAttacking = false;

        leftMeleeCollider.SetActive(false);

    }

    IEnumerator whip()
    {
        isAttacking = true;
        rightMeleeCollider.SetActive(true);
        anime.SetTrigger("Whip");
        
        yield return new WaitForSeconds(whipDelay);
        isAttacking = false;

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
    public void hurtBaddies(int amount)
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
            StartCoroutine(gameManager.instance.youWin());
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
        hurtBaddies(explosionDamage);
    }

}
