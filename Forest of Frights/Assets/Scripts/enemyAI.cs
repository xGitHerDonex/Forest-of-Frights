using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Device;

public class enemyAI : MonoBehaviour, IDamage
{
    #region Fields, Members and Variables
    [Header("-----Components-----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anime;

    [Header("-----Enemy Stats-----")]
    [Range(1, 10)][SerializeField] int hp;
    [Range(1, 10)][SerializeField] int targetFaceSpeed;
    [Range(-360, 360)][SerializeField] int viewAngle;
    [Range(0, 100)][SerializeField] int roamDistance;
    [Range(1, 10)][SerializeField] int roamPauseTime;
    [Range(1, 10)][SerializeField] float animeSpeedChange;

    [Header("-----Gun Stats and Bullet Component -----")]
    [Range(-360, 360)][SerializeField] int shootAngle;
    [Range(1, 10)][SerializeField] float shootRate;
    [SerializeField] GameObject bullet;

    Vector3 pushBack;
    Vector3 playerDirection;
    Vector3 startingPos;
    float stoppingDistOriginal;
    float angleToPlayer;

    bool playerInRange;
    bool isShooting;
    bool destinationPicked;

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        stoppingDistOriginal = agent.stoppingDistance;
        // Disabled updateGameGoal to not count the current enemies on screen
        //gameManager.instance.updateGameGoal(0);

    }


    /* 
     * if the player is in range of the enemy get the player position from the game manager instance running and subtract my enemies position from it for a direction
     * if the nav mesh distance between its current position and the player destination is than or equal to the stopping distance from the enemy
     * face the target
     * and if he isnt shooting then start the sub routine to shoot at the object
     * then the Nav mesh calcutates a new path to the destination if it has moved must return true 
     *  set destination otherwise returns false and no new path is calculated
     */
    // Update is called once per frame
    void Update()
    {
        //allows the enemy to ease into the transition animation with a tuneable
        //variable for custimization by Lerping it over time prevents choppy transitions
        float agentVel = agent.velocity.normalized.magnitude;
        
        anime.SetFloat("Speed", Mathf.Lerp(anime.GetFloat("Speed"), agentVel, Time.deltaTime * animeSpeedChange));

        //if the player is in range but cant be "seen" the enemy is allowed to roam
        //also if the player is not in range at all the enemy is allowed to roam
        if (playerInRange && !canSeePlayer())
        {
            StartCoroutine(roam());
        } else if (!playerInRange)
        {
            StartCoroutine(roam());
        }

    }



    IEnumerator roam()
    {
        //will allow the enemy to consulte with the nav mesh to pick a random destination that is walkable
        //that is within the roaming distance set that destinatino and walk to it
        if (agent.remainingDistance < 0.05f && !destinationPicked)
        {
            destinationPicked = true;
            yield return new WaitForSeconds(roamPauseTime);

            Vector3 randomPos = Random.insideUnitSphere * roamDistance;
            randomPos += startingPos;
            NavMeshHit destination;
            NavMesh.SamplePosition(randomPos, out destination, roamDistance, 1);
            agent.SetDestination(destination.position);

            destinationPicked = false;
        }
    } 



    /*
     * if the enemy takes damage requires an amount for the damage in a whole number
     * then subtracts the amount of damage from that whole number
     * the call the sub routine to run at the same time tomake the enemy feedback show (flashing damage indicator)
     * also if the health is less than or equal to 0 destroy this enemy
     * 
     */
    public void takeDamage(int amount)
    {
        hp -= amount;

        StartCoroutine(flashDamage());
        if (hp <= 0)
        {
            gameManager.instance.updateGameGoal(+1); //updates win condition set at 10 or greater "You win" also increments enemies killed and starts the win table Ienum
            Destroy(gameObject);
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



    //tests to see if the player is within range of the enemy and if the player is within range calculate
    /* the angle of the player to the enemy 
     * there is a debug to show the angle and the player position to the enemy position in the scene screen.
     * sends out a ray cast from the head of the enemy to the player to figure out the direction and if there is 
     * any obstacles in the way
     */
    bool canSeePlayer()
    {

        playerDirection = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);
        //will not compile in the release build
#if(UNITY_EDITOR)
        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDirection);
#endif

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            /*
             * if the ray cast hits an object and its the player and the angle to the player is less than
             * or equal to the preset viewing angle then tell the enemy to set the target destination to the player
             */
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.stoppingDistance = stoppingDistOriginal;
                agent.SetDestination(gameManager.instance.player.transform.position);
                /*
                 * if the remaining distance is less than or equal to the stopping  distance of the enemy
                 * face the target and prepare to shoot if the angle is within parameter and the enemy is not already shooting
                 * if these are true then start to shoot
                 */
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                    if (!isShooting && angleToPlayer <= shootAngle)
                    {
                        StartCoroutine(shoot());
                    }
                }
                return true;
            }
        }
        //otherwise set the stopping distance to zero and return false
        agent.stoppingDistance = 0;
        return false;
    } 

    // Set the shooting bool to true then
    // places and intializes the bullet object from the shooting postion and gives it a direction while triggering the bool that checks whether the enemy is shooting or not.
    //suspends the coroutine for the amount of seconds the shootrate is set to then sets the shooting back to false
    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
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
        agent.velocity += dir;
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
