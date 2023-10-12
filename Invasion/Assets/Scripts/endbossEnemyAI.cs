using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endbossEnemyAI : WayPatrolenemyAi
{

    // Update is called once per frame
    protected override void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            //allows the enemy to ease into the transition animation with a tuneable
            //variable for custimization by Lerping it over time prevents choppy transitions
            float agentVel = agent.velocity.normalized.magnitude;

            anime.SetFloat("Speed", Mathf.Lerp(anime.GetFloat("Speed"), agentVel, Time.deltaTime * animeSpeedChange));

            //if the player is in range but cant be "seen" the enemy is allowed to roam
            //also if the player is not in range at all the enemy is allowed to roam
            if (playerInRange && !canSeePlayer())
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
            }
           
        }
    }
}
