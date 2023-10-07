using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Device;

public class midBossAI : MonoBehaviour, IDamage, IPhysics
{
    [Header("-----Components-----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;
    [SerializeField] Animator anime;
    [SerializeField] Collider hitBox;

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


    [Header("-----EnergyBall Components-----")]
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
    bool isRunning;
    bool isShooting;
  

    // Start is called before the first frame update
    void Start()
    {
        speedOrig = agent.speed; // gives the agent speed to the float original speed for later on. 
        startingPos = transform.position; //saves starting pos
        stoppingDistOriginal = agent.stoppingDistance; // saves original stopping distance
        hp = maxHp; // sets hp
        isDead = false; // set's alive status

    }


    // Update is called once per frame
    void Update()
    {
        float hpRatio = (float)(hp / maxHp);

        //Selects stage for enemy AI based on Health Remaining
        if (hpRatio >= 0.4)
        {
            Stage1();
        }

        else
        {
            Stage2();
        }

    }


    void Stage1()
    {
        //If enemy is not dead, we'll continue
        if (!isDead)
        {
            //Continually determines enemy speed and run / walk animations
            updateRunningSpeed();

            //casts a ray on the player
            RaycastHit hit;

            //get's player's _direction
            playerDirection = gameManager.instance.player.transform.position - headPos.position;


            //Casts a ray on the player
            if (Physics.Raycast(headPos.position, playerDirection, out hit))
            {
                agentVel = agent.velocity.normalized.magnitude;
                anime.SetFloat("Speed", Mathf.Lerp(anime.GetFloat("Speed"), agentVel, Time.deltaTime * animeSpeedChange));

                //Gets distance to player
                distToPlayer = Vector3.Distance(headPos.position, gameManager.instance.player.transform.position);


                //If player within stopping distance, face target and attack if not already attacking
                if (!isAttacking && !isShooting && playerInRange && hit.collider.CompareTag("Player") && distToPlayer <= agent.stoppingDistance)
                {
                    
                        agent.velocity = Vector3.zero;
                        agent.ResetPath();
                        faceTarget();

                        if (!isRunning)
                            StartCoroutine(Melee(Random.Range(2,5)));
                   
                }

                //if player is not wthin stopping distance, then set distination to the player
                else if (playerInRange && hit.collider.CompareTag("Player") && distToPlayer >= agent.stoppingDistance)
                {

                    faceTarget();
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
            //Increases Melee Damage to double
            meleeDamage = meleeStage2Damage;

            //Continually determines enemy speed and run / walk animations
            updateRunningSpeed();


            //casts a ray on the player
            RaycastHit hit;

            //get's player's _direction
            playerDirection = gameManager.instance.player.transform.position - headPos.position;

            angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);

            // Runs if we cast a ray on the player
            if (Physics.Raycast(headPos.position, playerDirection, out hit))
            {
                agentVel = agent.velocity.normalized.magnitude;
                anime.SetFloat("Speed", Mathf.Lerp(anime.GetFloat("Speed"), agentVel, Time.deltaTime * animeSpeedChange));

                //Gets distance to player
                distToPlayer = Vector3.Distance(headPos.position, gameManager.instance.player.transform.position);

             
                //Uncomment to check distance between enemy player
                //Debug.Log(distToPlayer);

                   
                //If we aren't shooting or attacking, the player is Range, and the distance to the player is greater than the ranged Stopping distance
                if (!isAttacking && !isShooting && playerInRange && hit.collider.CompareTag("Player")  && distToPlayer >= rangedStoppingDistance && distToPlayer <= maxShootingRange)
                {
                    //set the agent
                    agent.stoppingDistance = rangedStoppingDistance;

                    if (distToPlayer >= agent.stoppingDistance)
                    {
                        agent.velocity = Vector3.zero;
                        agent.ResetPath();
                        faceTarget();                     
                        StartCoroutine(shoot());
                        agent.SetDestination(gameManager.instance.player.transform.position);

                    }
                }


                //Otherwise we try to reach the player and do a melee combo
                else
                {
                    agent.stoppingDistance = stoppingDistOriginal;


                    if (!isAttacking && !isShooting && playerInRange && hit.collider.CompareTag("Player") && distToPlayer <= agent.stoppingDistance)
                    {                     
                            agent.velocity = Vector3.zero;
                            agent.ResetPath();
                            faceTarget();

                            if (!isRunning)
                                StartCoroutine(Melee(Random.Range(2, 5)));
  

                    }

                    //if player is not wthin stopping distance, then set distination to the player
                    else if (playerInRange && hit.collider.CompareTag("Player") && distToPlayer >= agent.stoppingDistance)
                    {

                                           
                        faceTarget();
                        if (!isAttacking)
                            agent.SetDestination(gameManager.instance.player.transform.position);
                    }

                    else
                    {

                        faceTarget();
                        agent.SetDestination(gameManager.instance.player.transform.position);
                    }
                }

                 
            }
            

      

        }

   
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
            gameManager.instance.isMidBossDead();

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

    //Melee Routine that will generate a number at random to determine attack sequence.
    IEnumerator Melee(int combo)
    {
        isAttacking = true;
        leftMeleeCollider.SetActive(true);
        rightMeleeCollider.SetActive(true);

        if (combo == 2)
            anime.SetTrigger("Melee2");

        else if (combo == 3)
            anime.SetTrigger("Melee3");

        else if (combo == 4)
            anime.SetTrigger("Melee4");


    yield return new WaitForSeconds(meleeDelay);
        isAttacking = false;
        leftMeleeCollider.SetActive(false);
        rightMeleeCollider.SetActive(false);
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

    //sets the rotation of the enemy to face the player based on the player _direction to the enemy 
    //and it lerps the rotation over time so it is smooth and not choppy
    void faceTarget()
    {
        Quaternion rotation = Quaternion.LookRotation(playerDirection);
        //lerp over time rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * targetFaceSpeed);
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


    //if an object enters the collider for the enemy check to see if it is the Player
    //if it is the player set player in range bool to true
    //returns nothing
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

        }
    }



    // does the exact opposite as On Trigger enter
    // it checks to see if the object that is in the collider is the player if it isnt then the player isnt in range
    //set the stopping distance to zero 
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;

        }
    }

    
}
