using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{

    // GameManager Class will run these functions:
    // 1. Pause Menu
    // 2. Restart
    // 3. Quit
    // 4. Resume
    // 5. Pause States


    public static gameManager instance;            // instance for gameManager
    public GameObject player;                      // player
    public playerController playerScript;           // player controller
    bool isPaused;                                 // bool for if game is paused - tracks pause state.

    [SerializeField] GameObject currentMenu;       // Selected Menu - will store the current menu that needs to be controlled
    [SerializeField] GameObject pauseMenu;         // Pause Menu
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject loseMenu;

    [SerializeField] GameObject enemy1;
    [SerializeField] GameObject enemy2;

    [SerializeField] RectTransform spawner;
    [SerializeField] float spawnRate;


    [SerializeField] int enemiesKilled;
    [SerializeField] int enemygoal;

    public GameObject playerSpawnPos;



    //Initializes before Application Runs
    void Awake()
    {
        instance = this; // set the the instance to this 
        player = GameObject.FindGameObjectWithTag("Player"); // set player to player with tag "player"
        playerScript = player.GetComponent<playerController>(); // set player controller to the player controller of player
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && currentMenu == null) //if esc or cancel button is hit, and currentMenu is null
        {
            pause(); //Pause the game
            currentMenu = pauseMenu; // set current menu to pause menu
            currentMenu.SetActive(isPaused); // show menu
        }

 
        //Spawn Enemies
        if (Time.time > (spawnRate))
        {
            spawnRate = spawnRate + Time.time;
            StartCoroutine(spawnEnemy(enemy1));
            StartCoroutine(spawnEnemy(enemy2));
        }

    }

    //Spawns an enemy
    IEnumerator spawnEnemy(GameObject enemy)
    {
        Instantiate(enemy, spawner.position, transform.rotation);
        yield return new WaitForSeconds(spawnRate);



    }

    //These methods will contain the logic for pausing and unpausing the game

    //Pause State
    public void pause()
    {
        Time.timeScale = 0; //set timescale to 0
        Cursor.visible = true; // show cusor
        Cursor.lockState = CursorLockMode.Confined; //confine cursor
        isPaused = !isPaused; // flip pool for isPaused


    }

    //unPause State
    public void unPause()
    {
        Time.timeScale = 1; //set timescale to 1
        Cursor.visible = false; // hide cursor
        Cursor.lockState = CursorLockMode.Locked; //lock cursor
        isPaused = !isPaused; // flip isPaused bool
        currentMenu.SetActive(isPaused); // set Pause Menu as current window;
        currentMenu = null;  // remove window

    }
    public void updateGameGoal(int amount)
    {
        enemiesKilled += amount;
        if (enemiesKilled == enemygoal)
        {
            StartCoroutine(youWin());
        }

    }
    IEnumerator youWin()
    {
        yield return new WaitForSeconds(1);
        pause();
        currentMenu = winMenu;
        currentMenu.SetActive(isPaused);
    }

    public void youLose()
    {
        pause();
        currentMenu = loseMenu;
        currentMenu.SetActive(isPaused);
    }
}
