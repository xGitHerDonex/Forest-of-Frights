using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;           // instance for gameManager

    [Header("-----Player-----")]
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;

    [Header("-----Player UI-----")]
    [SerializeField] GameObject playerHp;
    [SerializeField] Image playerHpBar;
    [SerializeField] GameObject playerStam;
    [SerializeField] Image playerLeftStamBar;
    [SerializeField] Image playerRightStamBar;

    [Header("-----Game Goal-----")]
    [SerializeField] TMP_Text enemiesRemaingText;  //Text for UI
    [SerializeField] int enemiesKilledWinCond;     //Sets win condition
    [SerializeField] int enemiesRemaining;         //Tracks how many enmies are left to win

    [Header("-----Menus-----")]
    [SerializeField] GameObject currentMenu;       // Selected Menu - will store the current menu that needs to be controlled
    [SerializeField] GameObject pauseMenu;         // Pause Menu
    [SerializeField] GameObject winMenu;           // Win Menu
    [SerializeField] GameObject loseMenu;          // Lose Menu
    [SerializeField] GameObject playerDamageFlash; // Flash Screen when player gets injured

    public bool isPaused;

    [Header("-----SFX-----")]
    [SerializeField] AudioSource natureSoundSource;
    [SerializeField] AudioClip natureSounds;

    [Header("----Accessible Values-----")]
    [Range(1, 5)] public int explosionDamage;


    //Initializes before Application Runs
    void Awake()
    {
        instance = this; // set the the instance to this
        player = GameObject.FindGameObjectWithTag("Player"); // set player to player with tag "player"
        playerScript = player.GetComponent<playerController>(); // set player controller to the player controller of player
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos"); //sets player spawn pos



        //Adds some ambience
        if (natureSoundSource != null && natureSounds != null)
        {
            natureSoundSource.clip = natureSounds;
            natureSoundSource.loop = true;
            natureSoundSource.Play();
        }

    }

    private void Start()
    {
        //Set's the enemies remaining to the win condition
        enemiesRemaining = enemiesKilledWinCond;
        enemiesRemaingText.text = enemiesRemaining.ToString("0");
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
        //Updates the Enemies Remaining in the UI
        enemiesRemaining -= amount;
        enemiesRemaingText.text = enemiesRemaining.ToString("0");

        if (enemiesRemaining <= 0)
        {
            StartCoroutine(youWin());
        }

    }




    //Pulls up the Win table after 1 second of
    IEnumerator youWin()
    {
        currentMenu = winMenu;
        currentMenu.SetActive(isPaused);
        yield return new WaitForSeconds(1);
        pause();

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

    public int getExplosionDamage()
    {
        return explosionDamage;
    }
}
