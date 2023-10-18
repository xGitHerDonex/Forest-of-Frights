using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;           // instance for gameManager

    [Header("-----Components-----")]
    [SerializeField] GameObject checkPointMenu;


    [Header("-----Player-----")]
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;

    [Header("-----Player UI-----")]
    [SerializeField] GameObject playerHp;
    [SerializeField] Image playerHpBar;
    [SerializeField] GameObject playerStam;
    [SerializeField] Image playerStamBar;
    //[SerializeField] Image playerRightStamBar;
    [SerializeField] Image playerChronoBar;
    [SerializeField] Text grenadeCDText;


    [SerializeField] GameObject reticle;


    [Header("-----Game Goal-----")]
    [SerializeField] GameObject demonLordGameObject;
    public endBossAI demonLord;
    public midBossAI goro;
    public bool midBoss;
    public bool endBoss;
    public checkPoint midBossCP;



    [Header("-----Menus-----")]
    [SerializeField] GameObject currentMenu;       // Selected Menu - will store the current menu that needs to be controlled
    public GameObject pauseMenu;         // Pause Menu
    [SerializeField] GameObject winMenu;           // Win Menu
    [SerializeField] GameObject loseMenu;          // Lose Menu
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject inventoryMenu;
    [SerializeField] GameObject beginMenu;    // Inventory Menu
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
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");


        player.transform.position = playerSpawnPos.transform.position;


                  
    }

    private void Start()
    {
        if(beginMenu != null)
        {
            pause();
        }

        else
            isPaused = false;


    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && currentMenu == null) //if esc or cancel button is hit, and currentMenu is null
        {
            pause(); //Pause the game
            currentMenu = pauseMenu; // set current menu to pause menu
            currentMenu.SetActive(isPaused); // show menu
        }

        //if (Input.GetButtonDown("Inventory") && currentMenu == null) //if tab button is hit, and currentMenu is null
        //{
        //    pause(); //Pause the game
        //    currentMenu = inventoryMenu; // set current menu to pause menu
        //    currentMenu.SetActive(isPaused); // show menu
        //}


    }



    //Pause State
    public void pause()
    {
        Time.timeScale = 0; //set timescale to 0
        Cursor.visible = true; // show cusor
        Cursor.lockState = CursorLockMode.Confined; //confine cursor
        isPaused = !isPaused; // flip pool for isPaused
        reticle.SetActive(false);


    }

    //unPause State
    public void unPause()
    {
        Time.timeScale = 1; //set timescale to 1
        Cursor.visible = false; // hide cursor
        Cursor.lockState = CursorLockMode.Locked; //lock cursor
        isPaused = !isPaused; // flip isPaused bool
        currentMenu.SetActive(false); // set Pause Menu as current window;
        currentMenu = null;  // remove window
        reticle.SetActive(true); // Adds reticle back on screen

    }

    //Pulls up the Win table after 1 second of
    public IEnumerator youWin()
    {
        currentMenu = winMenu;
        currentMenu.SetActive(true);
        yield return new WaitForSeconds(5f);
        pause();

        currentMenu.SetActive(true);
    }
    //Pulls up the Lose Table after the player dies.
    public void youLose()
    {
        pause();
        currentMenu = loseMenu;
        currentMenu.SetActive(true);
    }

    //When called updates the fill level of the Hp Bar
    public void updateHpBar(float amount)
    {
        playerHpBar.fillAmount = amount;

    }

    //When called updates the fill level of the Stam Bar
    public void updateStamBar(float amount)
    {
        playerStamBar.fillAmount = amount;
        //playerRightStamBar.fillAmount = amount;

    }

    public void updateChronoBar(float amount)
    {

        playerChronoBar.fillAmount = amount;

    }

    public void updateGrenadeCDText(string text)
    {
        // if its F, it should show green to signal the player
        if (text == "F")
        {

            grenadeCDText.text = "<color=green>" + text + "</color>";
        }
        else
        {
            // if its counting down, it should show the default orange
            grenadeCDText.text = text;
        }
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


    //Determines final boss coming in
    public void isMidBossDead(bool isDead = true)
    {
        midBoss = isDead;
    }

    //activates finalboss
    public void activateBoss()
    {
        midBoss = true;
        demonLordGameObject.SetActive(true);
    }


    public IEnumerator checkPointPopup()
    {
        checkPointMenu.SetActive(true);
        yield return new WaitForSeconds(2);
        checkPointMenu.SetActive(false);
    }

    public void setControlsMenuActive(bool active = true)
    {
        if (active)
        {
            controlsMenu.SetActive(true);
        }

        else
        {
            controlsMenu.SetActive(false);
        }

    }

    public void restartMidBoss()
    {
        goro.resetFight();
    }

    public void restartEndBoss()
    {
        demonLord.resetFight();
    }



}


