using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Device;
using UnityEngine.ProBuilder;

public class endBossAI : MonoBehaviour
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



    [SerializeField] GameObject waypoint;
    [SerializeField] int flightSpeed;

 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        setFlightAnimation();

        if (rb.position != waypoint.transform.position)
        {
            headToTarget(waypoint);
        }

        
    }

    void headToTarget(GameObject target)
    {
        faceTarget(target);
        Vector3 position = Vector3.MoveTowards(rb.position, target.transform.position, flightSpeed * Time.deltaTime);
        rb.MovePosition(position);

    }


    void setFlightAnimation()
    {


        if (rb.position.y > 0 && rb.velocity.x > 0)
        {
            anime.SetBool("FlyRight", true);
        }

        else if(rb.position.y > 0 && rb.velocity.x < 0)
        {
            anime.SetBool("FlyLeft", true);
        }


        else if (rb.position.y > 0 && rb.velocity.x == 0)
        {
            anime.SetBool("Fly", true);
        }

        else
        {
            anime.SetBool("Fly", false);
            anime.SetBool("FlyLeft", false);
            anime.SetBool("FlyRight", false);
        }

       
           
        
    }


    void faceTarget(GameObject target)
    {
        Vector3 targetDirection = target.transform.position - headPos.position;
        Quaternion rotation = Quaternion.LookRotation(targetDirection);

        //lerp over time rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * targetFaceSpeed);
    }

}
