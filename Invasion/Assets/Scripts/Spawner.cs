using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    //Spawner will handle spawn points for enemies

    [SerializeField] GameObject enemy;

    [SerializeField] RectTransform spawner; //location of spawner
    [SerializeField] float spawnRate;       //spawn rate

    // Update is called once per frame
    void Update()
    {
        
        //Spawn Enemies
        if (Time.time > (spawnRate))
        {
            spawnRate = spawnRate + Time.time;
            StartCoroutine(spawnEnemy(enemy));
        }
    }

    //Spawns an enemy
    IEnumerator spawnEnemy(GameObject en)
    {
        //Vector3 spawnPosition = new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f));
        Instantiate(en, (spawner.position + new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f))), transform.rotation);
        yield return new WaitForSeconds(spawnRate);

    }
    
}
