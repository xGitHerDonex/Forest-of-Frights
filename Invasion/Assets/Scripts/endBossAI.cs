using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Device;


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

    [Tooltip("Enemy health value between 1 and 100.")]
    [Range(1, 1000)][SerializeField] float hp;
    [Range(1, 1000)][SerializeField] float maxHp;


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
    public int meleeDamage;
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
    [SerializeField] int airTime;


    [Header("-----Bools-----")]
    [SerializeField] bool startFight;
    [SerializeField] bool isLanding;
    [SerializeField] bool isAttacking;
    [SerializeField] bool isRunning;
    [SerializeField] bool isShooting;
    [SerializeField] bool isGrounded;
    [SerializeField] bool timeToLand;
    [SerializeField] bool isFlying;
    [SerializeField] bool reachedTarget;
    [SerializeField] bool isSummoning;
    [SerializeField] bool summonCompleted;
    [SerializeField] bool isRbDestroyed;
    public bool finalCheckpointActivated;


    [Header("-----Waypoints-----")]
    [SerializeField] float distToWaypoint;
    [SerializeField] float closestWaypointDist;
    [SerializeField] GameObject waypoint;
    [SerializeField] GameObject[] waypoints;
    [SerializeField] GameObject closestWaypoint;
    [SerializeField] float waypointDist;

    [Header("-----Summon Details-----")]
    [SerializeField] float summonTime;
    [SerializeField] int faceTargetWhileSummonDelay;


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


    [Header("-----Health Orb-----")]
    [SerializeField] GameObject healthOrb;
    [SerializeField] bool orb1;
    [SerializeField] bool orb2;
    [SerializeField] bool orb3;


    [Header("-----Summons-----")]
    [SerializeField] bool summon1;
    [SerializeField] bool summon2;
    [SerializeField] bool summon3;





    // Start is called before the first frame update
    void Start()
    {
        //Get's Player Script so we can check the closest waypoint

        startingPos = transform.position;
        playerScript = gameManager.instance.player.GetComponent<playerController>();
        origFlightSpeed = flightSpeed;
        stoppingDistOriginal = agent.stoppingDistance;
        isDead = false;
        isAttacking = false;
        isGrounded = true;
        isFlying = false;
        reachedTarget = false;
        summonCompleted = true;
        isSummoning = false;
        originalTargetFaceSpeed = targetFaceSpeed;

        //Creates Flight waypoint Matrix
        waypoints = GameObject.FindGameObjectsWithTag("FLWP");
    }



    // Update is called once per frame
    void Update()
    {
        if (!isDead && finalCheckpointActivated && playerInRange)
        {
            //agent.enabled = false;
            //Check HP levels
            float hpRatio = (hp / maxHp);

            //Drops orbs and calls summons based on remaining HP

            if (hpRatio <= .7f && !summon1 && !orb1)
            {

                Instantiate(healthOrb, transform.position + (Vector3.up * 1) + (Vector3.left * 2), transform.rotation);
                Instantiate(healthOrb, transform.position + (Vector3.up * 1), transform.rotation);
                orb1 = true;
                isSummoning = true;
                summon1 = true;
            }

            else if (hpRatio <= .5f && !summon2 && !orb2)
            {

                Instantiate(healthOrb, transform.position + (Vector3.up * 1) + (Vector3.left * 2), transform.rotation);
                Instantiate(healthOrb, transform.position + (Vector3.up * 1), transform.rotation);
                orb2 = true;
                isSummoning = true;
                summon2 = true;
            }

            else if (hpRatio <= .3f && !summon3 && !orb3)
            {

                Instantiate(healthOrb, transform.position + (Vector3.up * 1) + (Vector3.left * 2), transform.rotation);
                Instantiate(healthOrb, transform.position + (Vector3.up * 1), transform.rotation);
                orb3 = true;
                isSummoning = true;
                summon3 = true;
            }

            //Boss Phases' if Statements

            if (isSummoning)
            {

                summonRoutine(findClosestFlightWaypoint());
            }

            //Selects stage for enemy AI based on Health Remaining
            if (hpRatio >= 0.7f && !isSummoning)
            {
                Stage1();
            }

            else if (hpRatio < 0.8f  && !isSummoning)
            {

                if (timeToLand && isFlying && summonCompleted && !isGrounded)
                {
                    flyToGround(playerScript.getClosestGroundWaypoint());

                }

                else if (agent.isActiveAndEnabled)
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
 
                    agent.ResetPath();
                    agent.velocity = Vector3.zero;
                    facePlayer();
                    StartCoroutine(whip());


                }

                //If player within stopping distance, face target and attack if not already attacking
                else if (!isAttacking && !isShooting && hit.collider.CompareTag("Player") && distToPlayer <= meleeRange)
                {
 
                    agent.ResetPath();
                    agent.velocity = Vector3.zero;
                    facePlayer();
                    StartCoroutine(Melee(Random.Range(1, 5)));


                }
        

                else if (!isAttacking)
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

                    agent.ResetPath();
                    agent.velocity = Vector3.zero;
                    facePlayer();
                    StartCoroutine(whip());

                }

                //If player within stopping distance, face target and attack if not already attacking
                else if (!isAttacking && !isShooting && hit.collider.CompareTag("Player") && distToPlayer <= meleeRange)
                {

                    agent.ResetPath();
                    agent.velocity = Vector3.zero;
                    facePlayer();
                    StartCoroutine(Melee(Random.Range(1, 5)));

                }

                else if (!isAttacking && !isShooting && hit.collider.CompareTag("Player") && distToPlayer >= agent.stoppingDistance && distToPlayer <= maxShootingRange)
                {
                    agent.ResetPath();
                    agent.velocity = Vector3.zero;
                    facePlayer();
                    StartCoroutine(shoot());
                    //agent.SetDestination(gameManager.instance.player.transform.position);

                }

                else if (!isAttacking)
                {
                    facePlayer();
                    agent.SetDestination(gameManager.instance.player.transform.position);
                }
            }

        }
    }


    //once enemy flies up, this routine executes for summoning enemies
    void summonRoutine(GameObject waypoint)
    {
        
        if(!reachedTarget)
           flyToTarget(waypoint);

        else if (reachedTarget && isFlying)
        {
            faceTarget(gameManager.instance.playerScript.getClosestGroundWaypoint());
            StartCoroutine(faceTargetWhileSummon());
        }

    
     

    }

    //faces the ground waypoint when called.
    IEnumerator faceTargetWhileSummon()
    {
        rb.velocity = Vector3.zero;
        //agent.ResetPath();
        faceTarget(gameManager.instance.playerScript.getClosestGroundWaypoint());

        yield return new WaitForSeconds(faceTargetWhileSummonDelay); // Delay for boss to turn and face player before summon

        anime.SetTrigger("summon");
        summonCompleted = false;
        StartCoroutine(stayInAir()); //keeps Boss in Air for specified time
        StartCoroutine(summon()); //Summon logic
        reachedTarget = false;
    }

    //Heads to specified Target
    IEnumerator headToTarget(GameObject target, bool flyToGround, bool flight = true)
    {
        float distToTarget = Vector3.Distance(rb.position, target.transform.position);

    
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
            setFlightAnimation();
            isGrounded = false;
            isFlying = true;


            distToWaypoint = Vector3.Distance(rb.position, waypoint.transform.position);

            if (distToWaypoint <= groundingHeight)
            {
                isFlying = false;
                reachedTarget = true;
                isLanding = true;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, waypoint.transform.rotation, 90f);
                StartCoroutine(land());


             }

            else if (distToWaypoint >= groundingHeight)
            {   //faceTarget(gameManager.instance.playerScript.getClosestGroundWaypoint());
                isFlying = true;
                StartCoroutine(headToTarget(waypoint,true));
            }

           
        
    }

    //create rigid body and attaches back to the boss.
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

            setFlightAnimation();
            createRb();
            hitBox.enabled = false;

            isFlying = true;
            timeToLand = false;
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


    //Stay in Air timer
    IEnumerator stayInAir()
    {
        yield return new WaitForSeconds(airTime);

        timeToLand = true;
    }

    //Land Routine that will land the enemy and reenable nav mesh
    IEnumerator land()
    {
        
        if (isLanding)
        {

            if (!isGrounded && !isFlying)
            {
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
                hitBox.enabled = true;



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

        bossSpawnerManager.instance.setTimeToSpawn(true);
        yield return new WaitForSeconds(summonTime);
        bossSpawnerManager.instance.setTimeToSpawn(false);
        summonCompleted = true;
        isSummoning = false;



    }

    //Shoot ability
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

    //whip ability
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
        playAttackSound();

    }

    //plays enemy walking sound
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
        //StartCoroutine(stopMoving());


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
            //anime.SetTrigger("Damage");
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

    //Used for when player dies - resets boss and destorys relevant objects
    public void resetFight()
    {
        agent.SetDestination(startingPos);

        GameObject[] bossSpawns = GameObject.FindGameObjectsWithTag("artho");
        GameObject[] healthOrbs = GameObject.FindGameObjectsWithTag("healthOrb");

        foreach (GameObject enemy in bossSpawns)
        {
            Destroy(enemy);
        }

        foreach (GameObject healthOrb in healthOrbs)
        {
            Destroy(healthOrb);
        }

        summon1 = false;
        summon2 = false;
        summon3 = false;

        orb1 = false;
        orb2 = false;
        orb3 = false;

        hp = maxHp;
        playerInRange = false;



    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            hitBox.enabled = true;

        }
    }

}
