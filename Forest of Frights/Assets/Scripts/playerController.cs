using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage, IPhysics
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
    [Range(0, 2)][SerializeField] float jumpHeight;

    [Header("-----Expanded Player Stats-----")]
    [SerializeField] float originalPlayerSpeed;
    [SerializeField] int jumpsMax;
    [SerializeField] float gravityValue;
    [SerializeField] Vector3 pushBack;
    [SerializeField] float pushBackResolve;

    //Player basic shooting
    [Header("-----Gun Stats------")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] AudioClip shotSound;

    [Header("-----Grenade Stats------")]
    [SerializeField] GameObject grenade;
    [SerializeField] float throwRate;
    [Range(1, 20)][SerializeField] int playerThrowForce;
    [Range(1, 20)][SerializeField] int throwLift;
    [SerializeField] Transform throwPos;


    [Header("-----SFX-----")]
    //Player SFX
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip playerInjured;
    [SerializeField] AudioClip playerWalksGrass;
    [SerializeField] AudioClip playerRunsGrass;
    [SerializeField] AudioClip playerJumpsGrass;

   
    //Bools and others for functions
    private bool isShooting;
    private bool groundedPlayer;
    private bool canSprint = true;
    private bool isTakingDamage = false;
    private int jumpedTimes;
    private Vector3 playerVelocity;
    private Vector3 move;
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

        //Throw grenade
        //Expanded on this line to prevent the player from shooting during the pause menu (see gameManager bool)
        if (Input.GetButton("throw") && !isShooting && !gameManager.instance.isPaused)
            StartCoroutine(throwGrenade());

        //Keeps Stamina Bar updated
        gameManager.instance.updateStamBar(Stamina/ maxStamina);
       
    }

    //Move Ability:  Currently allows player to move!  Wheee!
    void movement()
    {
        //controls pushback amount
        if (pushBack.magnitude > 0.01f)
        {
                pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackResolve);
                pushBack.x = Mathf.Lerp(pushBack.x, 0, Time.deltaTime * pushBackResolve);
                pushBack.y = Mathf.Lerp(pushBack.y, 0, Time.deltaTime * pushBackResolve * 3);
                pushBack.z = Mathf.Lerp(pushBack.z, 0, Time.deltaTime * pushBackResolve);

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
            playerVelocity.y += jumpHeight; //Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            
            //Jumping now makes a sound
            audioSource.PlayOneShot(playerJumpsGrass);
            //Jumping now drains some stamina
            Stamina -= 1.1f;

        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity + pushBack * Time.deltaTime);
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
        audioSource.PlayOneShot(shotSound);

        //Casts a Ray and will hit object within range of the gun.
        //Also instantiates a particle effect if an enemy is hit
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();
            if (damageable != null)
            {
                Instantiate(gunList[selectedGun].hitEffect, hit.point, gunList[selectedGun].hitEffect.transform.rotation);
                damageable.takeDamage(shootDamage);
                
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator throwGrenade()
    {
        isShooting = true;

        //creates grenades
        GameObject thrownGrenade = Instantiate(grenade, throwPos.transform.position, throwPos.transform.rotation);  
        Rigidbody thrownGrenadeRb = thrownGrenade.GetComponent<Rigidbody>();

        //throws grenade
        thrownGrenadeRb.AddForce((throwPos.transform.forward * playerThrowForce * 20) + transform.up * throwLift, ForceMode.Impulse);

         
        yield return new WaitForSeconds(throwRate);
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

    //Physics method, player will not use bool here
    public void physics(Vector3 dir)
    {
        pushBack += dir;
    }

    //Updates players HP bar after a respawn.  Implemented in spawnPlayer()
    public void updatePlayerUI()
    {
        gameManager.instance.updateHpBar((float)HP/maxHP);
       
    }

    public void gunPickup(gunStats gun)
    {
        gunList.Add(gun);

        shootDamage = gun.shootDamage;
        shootDistance = gun.shootDist;
        shootRate = gun.shootRate;
        shotSound = gun.shotSound;

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
        shotSound = gunList[selectedGun].shotSound;
        shootDamage = gunList[selectedGun].shootDamage;
        shootDistance = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<Renderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<Renderer>().sharedMaterial;
    }

    //method made for delaying damage for physics
    public void delayDamage(int damage, float seconds)
    {
        StartCoroutine(delayedDamage(damage, seconds));
    }

    //method made for delaying damage for physics
    IEnumerator delayedDamage(int damage, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        takeDamage(damage);
    }
}
