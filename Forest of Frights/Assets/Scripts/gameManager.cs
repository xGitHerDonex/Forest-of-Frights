using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{

    // GameManager Class will run these functions:
    // 1. Pause Menu
    // 2. Restart
    // 3. Quit
    // 4. Resume
    // 5. Pause States
    // 6. Player HP
    // 7. Find Player Spawn


    public static gameManager instance;           // instance for gameManager

    [Header("-----Player-----")]
    public GameObject player;                     
    public playerController playerScript;        
    public GameObject playerSpawnPos;            
    private GameObject playerHp;
    [SerializeField] Image playerHpBar;                        
    private GameObject playerStam;
    [SerializeField] Image playerLeftStamBar;
    [SerializeField] Image playerRightStamBar;



    [Header("-----Menus-----")]
    [SerializeField] GameObject currentMenu;       // Selected Menu - will store the current menu that needs to be controlled
    [SerializeField] GameObject pauseMenu;         // Pause Menu
    [SerializeField] GameObject winMenu;           // Win Menu 
    [SerializeField] GameObject loseMenu;          // Lose Menu
    [SerializeField] GameObject playerDamageFlash; // Flash Screen when player gets injured
    [SerializeField] int enemiesKilled;            // Counts the Enemies that were killed
    
    public bool isPaused;                                 


    
    [Header("-----SFX-----")]
    [SerializeField] AudioSource natureSoundSource;
    [SerializeField] AudioClip natureSounds;



    //Initializes before Application Runs
    void Awake()
    {
        instance = this; // set the the instance to this 
        player = GameObject.FindGameObjectWithTag("Player"); // set player to player with tag "player"
        playerScript = player.GetComponent<playerController>(); // set player controller to the player controller of player
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos"); //sets player spawn pos 

       //playerHp = GameObject.FindWithTag("playerHp"); //Finds player HP bar
       //playerHpBar = playerHp.GetComponent<Image>(); //Sets player HP to that of Image component of the HP bar

       //playerStam = GameObject.FindWithTag("playerStam"); //Finds player Stam bar
       //playerLeftStamBar = playerStam.GetComponent<Image>(); //Sets player Stam to that of Image component of the Stam bar
       //playerRightStamBar = playerStam.GetComponent<Image>(); //Sets player Stam to that of Image component of the Stam bar


        //Adds some ambience
        if (natureSoundSource != null && natureSounds != null)
        {
            natureSoundSource.clip = natureSounds;
            natureSoundSource.loop = true;
            natureSoundSource.Play();
        }
    
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && currentMenu == null) //if esc or cancel button is hit, and currentMenu is null
        {
            pause(); //Pause the game
            currentMenu = pauseMenu; // set current menu to pause menu
            currentMenu.SetActive(isPaused); // show menu
        }


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

    //Our Win Condition.
    public void updateGameGoal(int amount)
    {
        enemiesKilled += amount;
        if (enemiesKilled >= 10)
        {
            StartCoroutine(youWin());
        }

    }

    //Pulls up the Win table after 1 second of 
    IEnumerator youWin()
    {
        yield return new WaitForSeconds(1);
        pause();
        currentMenu = winMenu;
        currentMenu.SetActive(isPaused);
    }
    //Pulls up the Lose Table after the player dies.
    public void youLose()
    {
        pause();
        currentMenu = loseMenu;
        currentMenu.SetActive(isPaused);
    }

    //When called updates the fill level of the Hp Bar
    public void updateHpBar(float amount)
    {
       playerHpBar.fillAmount = amount;

    }

    //When called updates the fill level of the Stam Bar
    public void updateStamBar(float amount)
    {
        playerLeftStamBar.fillAmount = amount;
        playerRightStamBar.fillAmount = amount;

    }

    //Flash the screen when player gets damaged
    public IEnumerator playerFlashDamage()
    {
        playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerDamageFlash.SetActive(false);
    }
}
