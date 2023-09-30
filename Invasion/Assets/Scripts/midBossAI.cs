using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Device;

public class midBossAI : MonoBehaviour, IDamage, IPhysics
{
    [Header("-----Components-----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anime;
    [SerializeField] Collider hitBox;

    [Header("-----Enemy Stats-----")]

    [Tooltip("Turning speed 1-10.")]
    [Range(1, 10)][SerializeField] int targetFaceSpeed;


    [Tooltip("Enemy health value between 1 and 100.")]
    [Range(1, 100)][SerializeField] int hp;


    [Tooltip("10 is the default value for all current speeds. Changing this without adjusting Enemy Speed and nav mesh speed will break it!!!!")]
    [Range(-30, 30)][SerializeField] float animeSpeedChange;
    [SerializeField] int viewAngle;


    [Header("-----Melee Components-----")]
    [SerializeField] GameObject leftMeleeCollider;
    [SerializeField] GameObject rightMeleeCollider;
    [Tooltip("Delay Value between melee attacks in seconds")]
    [SerializeField] int meleeDelay;
    [Tooltip("meleeDamage")]
    [SerializeField] int meleeDamage;

    [Header("SFX")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip walkSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip deathSound;




    Vector3 pushBack;
    Vector3 playerDirection;
    Vector3 startingPos;
    float stoppingDistOriginal;
    float angleToPlayer;
    float speedOrig;
    bool isAttacking;
    bool isDead;
    bool playerInRange;
    bool destinationPicked;

    // Start is called before the first frame update
    void Start()
    {
        speedOrig = agent.speed; // gives the agent speed to the float original speed for later on. 
        startingPos = transform.position;
        stoppingDistOriginal = agent.stoppingDistance;
        isDead = false;

    }


    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            //allows the enemy to ease into the transition animation with a tuneable
            //variable for custimization by Lerping it over time prevents choppy transitions
            float agentVel = agent.velocity.normalized.magnitude;

            anime.SetFloat("Speed", Mathf.Lerp(anime.GetFloat("Speed"), agentVel, Time.deltaTime * animeSpeedChange));

            //Automatically locks onto player 
            RaycastHit hit;

            playerDirection = gameManager.instance.player.transform.position - headPos.position;
            angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);
        
            if (Physics.Raycast(headPos.position, playerDirection, out hit))
            {

                float distToPlayer = Vector3.Distance(headPos.position, gameManager.instance.player.transform.position);
                Debug.Log(distToPlayer);

                if (playerInRange && hit.collider.CompareTag("Player") && distToPlayer <= agent.stoppingDistance)
                {
                    faceTarget();
                    StartCoroutine(Melee());
                    //agent.stoppingDistance = stoppingDistOriginal;


                }

                else if (playerInRange && distToPlayer >= agent.stoppingDistance)
                {
                    faceTarget();
                    agent.SetDestination(gameManager.instance.player.transform.position);
                    //agent.stoppingDistance = 0;


                }
            }
                
        }
    }

        void playWalkSound()
        {
          if (audioSource != null && walkSound != null)
            {
                audioSource.clip = walkSound;
                audioSource.Play();
            }
        }


        /*
         * if the enemy takes damage requires an amount for the damage in a whole number
         * then subtracts the amount of damage from that whole number
         * the call the sub routine to run at the same time to make the enemy feedback show (flashing damage indicator)
         * also if the health is less than or equal to 0 destroy this enemy
         * 
         */
        public void takeDamage(int amount)
        {
            hp -= amount;
            StartCoroutine(stopMoving());


            if (hp <= 0)
            {
                isDead = true; //Will keep update method from running since Enemy is now dead
                hitBox.enabled = false; // turns off the hitbox so player isnt collided with the dead body
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
            yield return new WaitForSeconds(1);
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


        IEnumerator Melee()
        {
            isAttacking = true;
            leftMeleeCollider.SetActive(true);
            rightMeleeCollider.SetActive(true);
            anime.SetTrigger("Melee");

            yield return new WaitForSeconds(meleeDelay);
            isAttacking = false;
            leftMeleeCollider.SetActive(false);
            rightMeleeCollider.SetActive(false);
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

        //sets the rotation of the enemy to face the player based on the player direction to the enemy 
        //and it lerps the rotation over time so it is smooth and not choppy
        void faceTarget()
        {
            Quaternion rotation = Quaternion.LookRotation(playerDirection);
            //lerp over time rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * targetFaceSpeed);
        }

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
