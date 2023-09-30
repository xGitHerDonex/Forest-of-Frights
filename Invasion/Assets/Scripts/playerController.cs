using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    [SerializeField] int playerSpeed;
    [Range(0, 7)][SerializeField] float jumpHeight;

    [Header("-----Expanded Player Stats-----")]
    [SerializeField] int originalPlayerSpeed;
    [SerializeField] int jumpsMax;
    [SerializeField] float gravityValue;
    [SerializeField] Vector3 pushBack;
    [SerializeField] float pushBackResolve;

    //Player basic shooting
    [Header("-----Gun Stats------")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] Transform gunMuzzle;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] AudioClip shotSound;
    // Railgun Unique
    [SerializeField] GameObject Railbeam;

    [Header("-----Grenade Stats------")]
    [SerializeField] GameObject grenade;
    [SerializeField] float throwRate;
    [Range(1, 20)][SerializeField] int playerThrowForce;
    [Range(1, 20)][SerializeField] int throwLift;
    [SerializeField] Transform throwPos;

    [Header("-----Chronokinesis-----")]
    [SerializeField] bool isTimeSlowed;
    [Range(0, 2)][SerializeField] float timeSlowInSeconds;
    [Range(0, 2)][SerializeField] float timeSlowScale;
    [Range(1, 20)][SerializeField] int playerSlowSpeed;

    [Header("-----UI Elements-----")]
    [SerializeField] private TextMeshProUGUI lowHPWarnText;
    [SerializeField] private GameObject lowHealthWarnBG;


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
    private bool isJumping;
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

        //Low Health Warning
        lowHealthWarning();

        //Call to shoot
        //Expanded on this line to prevent the player from shooting during the pause menu (see gameManager bool)
        if (!gameManager.instance.isPaused && gunList.Count > 0 && Input.GetButton("Shoot") && !isShooting)
            StartCoroutine(shoot());

        //Throw grenade - works similar to shoot    
        if (!gameManager.instance.isPaused && Input.GetButton("throw") && !isShooting)
            StartCoroutine(throwGrenade());

        //Throw grenade - works similar to shoot    
        if (!gameManager.instance.isPaused && Input.GetButton("time") && !isTimeSlowed && move.magnitude <= 0.4f)
            StartCoroutine(chronokinesis());



        //Keeps Stamina Bar updated
        gameManager.instance.updateStamBar(Stamina / maxStamina);

    }

    //Move Ability:  Currently allows player to move!  Wheee!
    void movement()
    {
        //Prevents inadvertent jumping while the game is paused
        if (gameManager.instance.isPaused)
        {
            return;
        }
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
            isJumping = false;
        }

        //Calculates movement
        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;


        //Adds sound to player footsteps (only while walking)
        float moveMagnitude = move.magnitude;

        //Debug.Log(moveMagnitude);
        if (moveMagnitude > .3f && !audioSource.isPlaying && !isJumping)
        {
            audioSource.PlayOneShot(playerWalksGrass);
        }

        //moves controller based on movement
        controller.Move(move * Time.deltaTime * playerSpeed);


        //Jump Ability:  Currently allows player to jump
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsMax)
        {
            jumpedTimes++;
            playerVelocity.y += jumpHeight;
            isJumping = true;
            //Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);

            //Jumping now makes a sound
            audioSource.PlayOneShot(playerJumpsGrass);
            //Jumping now drains some stamina
            Stamina -= 1.1f;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move((playerVelocity + pushBack) * Time.deltaTime);
    }

    //Sprint Ability:  Increases run speed by 5 for 4 seconds
    //Future: Powerups to increase maxStamina for increased sprinting
    void sprint()
    {
        if (Input.GetButton("Sprint") && canSprint && !isTimeSlowed)
        {
            //Increase player run speed by 5
            playerSpeed = originalPlayerSpeed + 5;
            Stamina -= 1.0f * Time.deltaTime;

            //WIP Sprint Sound
            float moveMagnitude = move.magnitude;
            //audioSource.Stop();
            if (playerSpeed == 10 && !audioSource.isPlaying && moveMagnitude >= 0.4f)
            {
                audioSource.PlayOneShot(playerRunsGrass);
            }
            {
                if (Stamina <= 0.0)
                {
                    canSprint = false;
                    if (!isTimeSlowed) //test
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

                if (!isTimeSlowed) // test
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
        if (isShooting || gunList.Count == 0 || gameManager.instance.isPaused)
            yield break;
        isShooting = true;

        // Talk to gunStats to grab the current gun's Recoil
        float recoilAmount = gunList[selectedGun].recoilAmount;

        // Save the original gunModel rotation (to recoil back to)
        Quaternion originalRotation = gunModel.transform.localRotation;

        // Play the shoot sound
        audioSource.PlayOneShot(shotSound);

        // If statement to simulate a chargetime of 2 seconds
        if (gunList[selectedGun].isRailgun)
        {
            yield return new WaitForSeconds(2.0f);
        }

        // Cast a ray and check for hits
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();
            if (damageable != null && hit.transform != transform)
            {
                Instantiate(gunList[selectedGun].hitEffect, hit.point, gunList[selectedGun].hitEffect.transform.rotation);
                damageable.takeDamage(shootDamage);
            }
        }

        // Railgun shoot effect
        if (gunList[selectedGun].isRailgun)
        {
            // Find the gunMuzzle within the currently selected gun's hierarchy
            Transform gunMuzzle = gunList[selectedGun].model.transform.Find("gunMuzzle");

            // Check if the gunMuzzle was found
            if (gunMuzzle != null)
            {
                // Instantiate the projectile from the gunMuzzle's position and rotation
                GameObject railBeam = Instantiate(gunList[selectedGun].projectile, gunMuzzle.position, gunMuzzle.rotation);
                Rigidbody railBeamRb = railBeam.GetComponent<Rigidbody>();
                railBeamRb.velocity = gunMuzzle.transform.forward * gunList[selectedGun].projectileSpeed;
            }
        }

        Vector3 recoilForce = new Vector3(-recoilAmount, 0f, 0f);
        gunModel.transform.localEulerAngles += recoilForce;

        // Smoothly return the gunModel to its original rotation
        float elapsedTime = 0f;
        // returnDuration time can be adjusted for smoothness
        float returnDuration = 0.6f;
        while (elapsedTime < returnDuration)
        {
            gunModel.transform.localRotation = Quaternion.Lerp(gunModel.transform.localRotation, originalRotation, elapsedTime / returnDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // gunModel rotation gets back to the original position
        gunModel.transform.localRotation = originalRotation;

        // Wait for the shoot rate cooldown
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    //Throw Grenade: will throw a grenade based on the throwforce and lift provided. 
    //Is infinite but only 1 grenade per second can be thrown
    IEnumerator throwGrenade()
    {
        isShooting = true;

        //creates grenades
        GameObject thrownGrenade = Instantiate(grenade, throwPos.transform.position, throwPos.transform.rotation);
        Rigidbody thrownGrenadeRb = thrownGrenade.GetComponent<Rigidbody>();

        //throws grenade
        thrownGrenadeRb.AddForce((throwPos.transform.forward * playerThrowForce * 20) + (transform.up * throwLift), ForceMode.Impulse);

        //wait for throwrate, then flip bool back
        yield return new WaitForSeconds(throwRate);
        isShooting = false;

    }

    IEnumerator chronokinesis()
    {
        originalPlayerSpeed = playerSpeed;
        playerSpeed = playerSpeed * playerSlowSpeed;
        isTimeSlowed = true;
        Time.timeScale = timeSlowScale;

        yield return new WaitForSeconds(timeSlowInSeconds);

        //revert scale and update player speed
        Time.timeScale = 1f;
        playerSpeed = originalPlayerSpeed;
        isTimeSlowed = false;


    }


    IEnumerator delaySpeed()
    {
        playerSpeed = playerSpeed * 10000;
        yield return new WaitForSeconds(0.2f);
        //slow time and increase player speed

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

    private void lowHealthWarning()
    {
        //Once the player hits approximately 33% HP of their Max HP (equipment, etc) it'll trigger the warning
        float lowHPThresh = maxHP * 0.33f;

        if (HP <= lowHPThresh)
        {
            //Turn on Low Health warning
            lowHPWarnText.gameObject.SetActive(true);
            lowHealthWarnBG.SetActive(true);
        }
        else
        {
            //Turn off Low Health warning
            lowHPWarnText.gameObject.SetActive(false);
            lowHealthWarnBG.SetActive(false);
        }
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
        gameManager.instance.updateHpBar((float)HP / maxHP);

    }

    //Updates Stats on Player from Gun
    public void gunPickup(gunStats gun)
    {
        gunList.Add(gun);

        shootDamage = gun.shootDamage;
        shootDistance = gun.shootDist;
        shootRate = gun.shootRate;
        shotSound = gun.shotSound;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<Renderer>().sharedMaterial = gun.model.GetComponent<Renderer>().sharedMaterial;
        gunList[selectedGun].gunMuzzle = gunMuzzle;

        selectedGun = gunList.Count - 1;
    }

    //Selecting Gun Method
    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            // Sets current gun to not-Railgun
            gunList[selectedGun].isRailgun = false;

            selectedGun++;

            // Detects if current gun is Railgun
            if (gunList[selectedGun].isRailgun)
            {
                gunList[selectedGun].isRailgun = true;
            }


            changeGun();
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            // Sets current gun to not-Railgun
            gunList[selectedGun].isRailgun = false;
            
            selectedGun--;

            // Detects if current gun is Railgun
            if (gunList[selectedGun].isRailgun)
            {
                gunList[selectedGun].isRailgun = true;
            }
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
