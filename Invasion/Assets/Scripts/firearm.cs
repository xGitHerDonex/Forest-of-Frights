using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class firearm : MonoBehaviour
{
    public int damage = 1;
    public float range = 10f;
    public float force = 30f;
    public float firerate = 15f;
    public float specialRate = 1f;
    public float recoilAmount = 1f;


    public int maxAmmo = 10;
    public int currentAmmo;
    public float reloadTime = 1f;
    public float specialReload = 1f;
    private bool isReloading = false;
    private bool manualReload = false;

    // Pistol Special 3-Round Burst
    public bool inBurstMode = false;
    private int burstShotCount = 0;
    private int maxBurstShots = 3;
    public float burstCooldown = 5f;

    public Camera shootCam;
    public ParticleSystem muzzleFlash;
    public AudioClip shotSound;
    public bool antiGrenade = false;
    private bool specialUsed = false;
    IDamage player;

    public GameObject hitEffect;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject weaponHolder;
    [SerializeField] public Text ammoCurText;
    [SerializeField] public Text ammoMaxText;

    public enum FirearmType
    {
        Pistol,
        PulsarRifle,
        Shotgun,
    }
    public FirearmType firearmType;

    [SerializeField] weaponSwap _weaponSwap;

    private float nextShot = 0f;


    public Animator animator;
    private void Start()
    {
        player = gameManager.instance.playerScript.GetComponent<IDamage>();

        currentAmmo = maxAmmo;
        

    }

    private void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
        //isShooting = false;
        //animator.SetBool("isShooting", false);
    }

    private void Awake()
    {
        UpdateAmmoUI();
    }
    void Update()
    {
       // UpdateAmmoUI();
        if (isReloading)
            return;
        UpdateAmmoUI();
        if (currentAmmo <= 0 || manualReload)
        {
            StartCoroutine(Reload());
            manualReload = false;
            return;
        }

        if (Input.GetButton("Shoot") && Time.time >= nextShot)
        {

            animator.SetBool("isShooting", true);
            nextShot = Time.time + 1f / firerate;
            Shoot();

        }

        else
        {
            animator.SetBool("isShooting", false);
        }

        if (Input.GetButton("Special") && Time.time >= nextShot)
        {
            animator.SetBool("isShooting", true);
            nextShot = Time.time + 1f / specialRate;
            Special();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            manualReload = true;
        }
    }
    public void Shoot()
    {

        if (!gameManager.instance.isPaused)
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();

            }
            audioSource.PlayOneShot(shotSound);
            currentAmmo--;
            antiGrenade = true;
            RaycastHit hit;
            if (Physics.Raycast(shootCam.transform.position, shootCam.transform.forward, out hit, range))
            {

                IDamage damageable = hit.collider.GetComponent<IDamage>();

                if (damageable != player && damageable != null && hit.transform != transform && damageable != player)
                {
                    damageable.hurtBaddies(damage);
                    GameObject hitEffectGO = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(hitEffectGO, 1f);
                }

            }

            animator.SetBool("isShooting", true);
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * force);
            }

            //GameObject hitEffectGO = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            //Destroy(hitEffectGO, 1f);

            if (ammoCurText != null)
            {
                ammoCurText.text = currentAmmo.ToString();
            }
        }
    }

    public void Special()
    {
        if (!gameManager.instance.isPaused)
        {
            if (firearmType == FirearmType.Pistol && currentAmmo >= 3)
            {
                StartCoroutine(PistolBurst());
            }
            else
            {
                if (muzzleFlash != null)
                {
                    muzzleFlash.Play();

                }
                audioSource.PlayOneShot(shotSound);
                currentAmmo--;
                antiGrenade = true;

                RaycastHit hit;
                if (Physics.Raycast(shootCam.transform.position, shootCam.transform.forward, out hit, range))
                {

                    IDamage damageable = hit.collider.GetComponent<IDamage>();

                    if (damageable != player && damageable != null && hit.transform != transform && damageable != player)
                    {
                        damageable.hurtBaddies(damage);
                        GameObject hitEffectGO = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(hitEffectGO, 1f);
                    }

                }
                animator.SetBool("isShooting", true);
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * force);
                }

                //GameObject hitEffectGO = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                //Destroy(hitEffectGO, 1f);
                specialUsed = true;

                if (ammoCurText != null)
                {
                    ammoCurText.text = currentAmmo.ToString();
                }
            }
        }
    }
    IEnumerator PistolBurst()
    {
        inBurstMode = true;
        burstShotCount = 0;

        while (burstShotCount < maxBurstShots)
        {
            Shoot();
            burstShotCount++;

            if (burstShotCount < maxBurstShots)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Apply the cooldown after the burst is finished
        yield return new WaitForSeconds(burstCooldown);

        inBurstMode = false;
    }
    

    IEnumerator Reload()
    {
        if (currentAmmo == maxAmmo)
        {
            yield break;
        }
        isReloading = true;
        animator.SetBool("Reloading", true);
        ammoCurText.text = "Reload";


        if (specialUsed)
        {
            yield return new WaitForSeconds(specialReload - .25f);
        }
        else
        {
            yield return new WaitForSeconds(reloadTime - .25f);
        }

        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);

        currentAmmo = maxAmmo;
        if (ammoCurText != null)
        {
            ammoCurText.text = currentAmmo.ToString();
        }
        
        isReloading = false;
        specialUsed = false;
    }


    public void UpdateAmmoUI()
    {
        if (ammoCurText != null)
        {
            if (currentAmmo > 0)
            {
                ammoCurText.text = currentAmmo.ToString();
                ammoMaxText.text = maxAmmo.ToString();
            }
            else
            {
                //ammoCurText.text = "";
                ammoMaxText.text = maxAmmo.ToString(); 
            }
        }

    }

}
