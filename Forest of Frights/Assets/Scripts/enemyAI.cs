using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class enemyAI : MonoBehaviour, IDamage
{
    #region Fields, Members and Variables
    [Header("-----Components-----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    [Header("-----Enemy Stats-----")]
    [Range(1, 10)][SerializeField] float shootRate;
    [Range(1, 10)][SerializeField] int hp;
    [Range(1, 10)][SerializeField] int targetFaceSpeed;

    Vector3 playerDir;    
    Vector3 pushBack;
    Vector3 playerDirection;

    bool playerInRange;
    bool isShooting;
    #endregion




    // Start is called before the first frame update
    void Start()
    {
        //Disabled updateGameGoal to not count the current enemies on screen
        //gameManager.instance.updateGameGoal(0);

    }
    #region Update
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
        if (playerInRange)
        {

            playerDirection = gameManager.instance.player.transform.position - transform.position;


            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
            }

            agent.SetDestination(gameManager.instance.player.transform.position);
        }
    }

    #endregion

    #region if the enemy takes damage

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
    #endregion

    #region Enemy Feedback upon taking damage
    //changes the material color from the original material to a red color for .1 seconds
    //the changes the color back to its original white state.
    IEnumerator flashDamage()
    {
              
    
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
       
    } 
    #endregion

    #region Shoot
    // Set the shooting bool to true then
    // places and intializes the bullet object from the shooting postion and gives it a direction while triggering the bool that checks whether the enemy is shooting or not.
    //suspends the coroutine for the amount of seconds the shootrate is set to
    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    } 
    #endregion

    #region Face the Target

    //sets the rotation of the enemy to face the player based on the player direction to the enemy 
    //and it lerps the rotation over time so it is smooth and not choppy
    void faceTarget()
    {
        Quaternion rotation = Quaternion.LookRotation(playerDirection);
        //lerp over time rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * targetFaceSpeed);
    } 
    #endregion

    #region On Trigger Enter

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
    #endregion

    #region On Trigger Exit
    // does the exact opposite as On Trigger enter
    // it checks to see if the object that is in the collider is the player if it isnt the 
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

        }
    }
    #endregion
    public void physics(Vector3 dir)
    {
        agent.velocity += dir;
    }


}
