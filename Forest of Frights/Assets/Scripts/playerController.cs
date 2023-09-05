using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;

    //Players stats
    [SerializeField] int HP;
    [SerializeField] int jumpsMax;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] float playerSpeed;
    
    //Player basic shooting
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;

    //Bools and others for functions
    private bool isShooting;
    private bool groundedPlayer;
    private int jumpedTimes;
    private Vector3 playerVelocity;
    private Vector3 move;
    



    private void Start()
    {
        hpBar.fillamount
    }

    void Update()
    {
       //Call to movement
        movement();
       //Call to shoot
        if (Input.GetButton("Shoot") && !isShooting)
            StartCoroutine(shoot());
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


    // Jump Ability:  Currently allows player to jump
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsMax)
        {
            jumpedTimes++;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
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
    }
}
