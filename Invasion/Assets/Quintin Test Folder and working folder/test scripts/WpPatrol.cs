using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WayPointPatrol : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    public Transform[] waypoints;
    int m_PathIndex;
    public float animeSpeedChange;
    Animator anime;

    private void Awake()
    {
        anime = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        StartPatrol();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            m_PathIndex = (m_PathIndex + 1) % waypoints.Length;
            navMeshAgent.SetDestination(waypoints[m_PathIndex].position);
        }
    }
   
    void StartPatrol()
    {
        //just starts the enemy on the way to the initail waypoint.
        navMeshAgent.SetDestination(waypoints[0].position.normalized);

        //test code will integrate a default speed in enemy AI.
        anime.SetFloat("isMoving", 1);
       
    }



}
