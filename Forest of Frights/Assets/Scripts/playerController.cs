using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;

    //Players stats
    [SerializeField] float HP;
    [SerializeField] float maxHP;
    [SerializeField] float maxStamina; 
    [SerializeField] float regenStamina;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;

    //Player UI Bar
    [SerializeField] Image hpBar;


    //Expanded Player stats
    [Header("Expanded Player Stats")]
    [SerializeField] float originalPlayerSpeed;
    [SerializeField] float drainStamina;
    [SerializeField] int jumpsMax; 
    [SerializeField] float gravityValue;
   
    //Player basic shooting
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;

    //Bools and others for functions
    private bool isShooting;
    private bool groundedPlayer;
    private bool canSprint = true;
    private int jumpedTimes;
    private Vector3 playerVelocity;
    private Vector3 move;
    float OriginalHp;
    



    private void Start()
    {
        OriginalHp = maxHP;
        spawnPlayer();
        originalPlayerSpeed = playerSpeed;
        hpBar.fillAmount = HP / maxHP;
          
    }

    void Update()
    {
       //Call to movement
        movement();
        sprint();

       //Call to shoot
        if (Input.GetButton("Shoot") && !isShooting)
            StartCoroutine(shoot());
       
       //Updates Fill on HP
       hpBar.fillAmount = HP / maxHP;
    }

    //Move Ability:  Currently allows player to move!  Wheee!
    void movement()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            jumpedTimes = 0;
            playerVelocity.y = 0f;
        }
        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(move * Time.deltaTime * playerSpeed);


    //Jump Ability:  Currently allows player to jump
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsMax)
        {
            jumpedTimes++;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    //Jumping now drains some stamina
            drainStamina -= 1.1f;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    //Sprint Ability:  Increases run speed by 4 for 4 seconds
    //Future: Powerups to increase maxStamina for increased sprinting
    void sprint()
    {
        if (Input.GetButton("Sprint") && canSprint)
        {
            //isSprinting = true;
            playerSpeed = originalPlayerSpeed + 4;
            drainStamina -= 1.0f * Time.deltaTime;
            {
                if (drainStamina <= 0.1)
                {
                    //isSprinting = false;
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
            if (drainStamina >= 4.0)
            {
                canSprint = true;
                playerSpeed = originalPlayerSpeed;

            }
            drainStamina += regenStamina * Time.deltaTime;
            //Clamp prevents stamina going negative or over the max
            drainStamina = Mathf.Clamp(drainStamina, 0, maxStamina);
        }

    }


    //Shoot Ability:  Currently instant projectile speed
    IEnumerator shoot()
    {
        isShooting = true;

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
        HP -= amount;
        if(HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }
    public void spawnPlayer() 
    {
        HP = OriginalHp;
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }
}
