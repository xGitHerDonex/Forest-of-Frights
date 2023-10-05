using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossSpawnerManager : MonoBehaviour
{
    public static bossSpawnerManager instance;
    [SerializeField] GameObject[] spawners;
    [SerializeField] bool timeToSpawn;
    [SerializeField] int spawnRate;
    [SerializeField] int spawnWaves;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        spawners = GameObject.FindGameObjectsWithTag("spawner");
    }

    public void spawnWave()
    {
    }

    public bool getTimeToSpawn()
    {
        return timeToSpawn;
    }

    public void setTimeToSpawn(bool _timeToSpawn)
    {
        timeToSpawn = _timeToSpawn;
    }

    public int getWavesToSpawn()
    {
        return spawnWaves;
    }

    public int getSpawnRate()
    {
        return spawnRate;
    }
}
