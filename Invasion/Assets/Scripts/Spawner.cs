using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{

    //Spawner will handle spawn points for enemies

    [SerializeField] GameObject enemy;
    [SerializeField] float spawnRate;       //spawn rate
    [SerializeField] int wavesToSpawn;      //how many waves
    bool isSpawning;                        //currently spawning

    private void Start()
    {
        //wavesToSpawn = bossSpawnerManager.instance.getWavesToSpawn();
        //spawnRate = bossSpawnerManager.instance.getSpawnRate();
    }

    // Update is called once per frame
    void Update()
    {

        if (bossSpawnerManager.instance.getTimeToSpawn())
        {
            StartCoroutine(spawnEnemy(enemy));
        }

    }

    //Spawns an enemy
   public IEnumerator spawnEnemy(GameObject en)
    {
        if(!isSpawning)
        {
            isSpawning = true;
            Instantiate(en, (transform.position + new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f))), transform.rotation);
            wavesToSpawn--;
            yield return new WaitForSeconds(spawnRate);
            isSpawning=false;
        }


    }
    
}
