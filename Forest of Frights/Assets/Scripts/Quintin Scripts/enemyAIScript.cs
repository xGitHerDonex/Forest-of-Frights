using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    bool isShooting;


    [SerializeField] int hp;
    [SerializeField] int targetFaceSpeed;

    bool playerInRange;


    Vector3 playerDirection;




    // Start is called before the first frame update
    void Start()
    {


    }

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

    public void takeDamage(int amount)
    {
        hp -= amount;
        StartCoroutine(flashDamage());
        if (hp <= 0)
        {
            Destroy(gameObject);
        }

    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void faceTarget()
    {
        Quaternion rotation = Quaternion.LookRotation(playerDirection);
        //lerp over time rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * targetFaceSpeed);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

        }
    }


}
