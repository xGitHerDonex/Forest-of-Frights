using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage,IPhysics
{
    [SerializeField] CharacterController controller;

    //Players stats
    [Header("-----Player Stats-----")]
    [SerializeField] float HP;
    [SerializeField] float maxHP;
    [SerializeField] float maxStamina;
    [SerializeField] float Stamina;
    [SerializeField] float regenStamina;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;


    //int OriginalHp;

    [Header("-----PushBack Tweak-----")]
    
    [SerializeField] float pushBackResolve;

    //Player basic shooting
    [Header("-----Gun Stats------")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;

    [Header("-----SFX-----")]
    //Player SFX
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip playerInjured;
    //[SerializeField] AudioClip playerShoot;
    [SerializeField] AudioClip playerWalksGrass;
    [SerializeField] AudioClip playerRunsGrass;
    [SerializeField] AudioClip playerJumpsGrass;

    //Expanded Player stats
    [Header("-----Expanded Player Stats-----")]
    [SerializeField] float originalPlayerSpeed;
    [SerializeField] int jumpsMax; 
    [SerializeField] float gravityValue;

    

    //Bools and others for functions
    private bool isShooting;
    private bool groundedPlayer;
    private bool canSprint = true;
    private bool isTakingDamage = false;
    private int jumpedTimes;
    private Vector3 playerVelocity;
    private Vector3 move;
    private Vector3 pushback;
    int selectedGun;


    private void Start()
    {
        //set up for respawn
        HP = maxHP;
        Stamina = maxStamina;
        spawnPlayer();

        //Sets player speed
        originalPlayerSpeed = playerSpeed;

    }

    void Update()
    {
       //Call to movement
        movement();
        sprint();

       //Use Selected Gun
        selectGun();

        //Call to shoot
        //Expanded on this line to prevent the player from shooting during the pause menu (see gameManager bool)
        if (Input.GetButton("Shoot") && !isShooting && !gameManager.instance.isPaused)
            StartCoroutine(shoot());

        //Keeps Stamina Bar updated
        gameManager.instance.updateStamBar(Stamina/ maxStamina);
       
    }

    //Move Ability:  Currently allows player to move!  Wheee!
    void movement()
    {
        //controls pushback amount
        if (pushback.magnitude > 0.01f)
        {
                pushback = Vector3.Lerp(pushback, Vector3.zero, Time.deltaTime * pushBackResolve);
                pushback.x = Mathf.Lerp(pushback.x, 0, Time.deltaTime * pushBackResolve);
                pushback.y = Mathf.Lerp(pushback.y, 0, Time.deltaTime * pushBackResolve * 3);
                pushback.z = Mathf.Lerp(pushback.z, 0, Time.deltaTime * pushBackResolve);

        }

        //Sets grounded bool if the player is grounded
        groundedPlayer = controller.isGrounded;

        //Resets jump and updates player vertical vecloity is player is grounded
        if (groundedPlayer && playerVelocity.y < 0)
        {
            jumpedTimes = 0;
            playerVelocity.y = 0f;
        }

        //Calculates movement
        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;


        //Adds sound to player footsteps (only while walking)
        float moveMagnitude = move.magnitude;

        //Debug.Log(moveMagnitude);
        if (moveMagnitude >.4f && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(playerWalksGrass);
        }

        //moves controller based on movement
        controller.Move(move * Time.deltaTime * playerSpeed);


    //Jump Ability:  Currently allows player to jump
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsMax)
        {
            jumpedTimes++;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            
            //Jumping now makes a sound
            audioSource.PlayOneShot(playerJumpsGrass);
            //Jumping now drains some stamina
            Stamina -= 1.1f;

        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    //Sprint Ability:  Increases run speed by 5 for 4 seconds
    //Future: Powerups to increase maxStamina for increased sprinting
    void sprint()
    {
        if (Input.GetButton("Sprint") && canSprint)
        {         
            //Increase player run speed by 5
            playerSpeed = originalPlayerSpeed + 5;
            Stamina -= 1.0f * Time.deltaTime;
            
            //WIP Sprint Sound
            float moveMagnitude = move.magnitude;
            //audioSource.Stop();
            if (moveMagnitude >= 0.1f && playerSpeed == 10 && !audioSource.isPlaying)
            {
                
                audioSource.PlayOneShot(playerRunsGrass);
            }
            {
                if (Stamina <= 0.1)
                {             
                    canSprint = false;
                    playerSpeed = originalPlayerSpeed;
                }
            }
        }

    //Stamina Recover Ability:  Recovers stamina by 0.75(current regenStamina)
    //Future: Powerups to speed up regenStamina
    //Note: drainStamina set to 4.0 to allow sprinting sooner than possible maxStamina
        else
        {
            if (Stamina >= 4.0)
            {
                canSprint = true;
                playerSpeed = originalPlayerSpeed;

            }
            Stamina += regenStamina * Time.deltaTime;
            //Clamp prevents stamina going negative or over the max

            Stamina = Mathf.Clamp(Stamina, 0, maxStamina);
        }

    }


    //Shoot Ability:  Currently instant projectile speed
    IEnumerator shoot()
    {
        isShooting = true;
        
        //Shoot Sound!
        //audioSource.PlayOneShot(playerShoot);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(shootDamage);
                
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    //Heal Ability:  Currently through the pause menu, until medkits are implemented
    public void giveHP(int amount)
    {
        HP += amount;
    }

    //Damageable Ability:  Currently allows player takes damage
    public void takeDamage(int amount)
    {
        if (!isTakingDamage)
        {
            isTakingDamage = true;
            HP -= amount;
            gameManager.instance.updateHpBar(HP / maxHP); // updates HP UI
            StartCoroutine(gameManager.instance.playerFlashDamage());

            if (HP <= 0)
            {
                //Plays the you lose method after your health hits 0
                gameManager.instance.youLose();
            }
            else
            {
                //Damage sound!
                audioSource.PlayOneShot(playerInjured);
            }
            StartCoroutine(ResetTakingDamage());
        }
    }


    IEnumerator ResetTakingDamage()
    {
        //Small delay to prevent repeat damage
        yield return new WaitForSeconds(0.1f);
        isTakingDamage = false;
    }
    public void spawnPlayer() 
    {
       
        //Resets Players HP
        HP = maxHP;       
        updatePlayerUI();

        //Prevents playerController from taking over the script
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }
    public void physics(Vector3 dir)
    {
        pushback += dir;
    }

    //Updates players HP bar after a respawn.  Implemented in spawnPlayer()
    public void updatePlayerUI()
    {
        gameManager.instance.updateHpBar((float)HP/maxHP);
        //gameManager.instance.playerHpBar.fillAmount = (float)maxHP / HP;
    }

    public void gunPickup(gunStats gun)
    {
        gunList.Add(gun);

        shootDamage = gun.shootDamage;
        shootDistance = gun.shootDist;
        shootRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<Renderer>().sharedMaterial = gun.model.GetComponent<Renderer>().sharedMaterial;

        selectedGun = gunList.Count - 1;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            changeGun();
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            changeGun();
        }
    }

    void changeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootDistance = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<Renderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<Renderer>().sharedMaterial;
    }
}
