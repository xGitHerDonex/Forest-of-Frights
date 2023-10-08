using System.Collections;
using UnityEngine;

public class firearm : MonoBehaviour
{
    public int damage = 1;
    public float range = 10f;
    public float force = 30f;
    public float firerate = 15f;
    public float specialRate = 1f;

    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f;
    public float specialReload = 1f;
    private bool isReloading = false;
    

    public Camera shootCam;
    public ParticleSystem muzzleFlash;
    public AudioClip shotSound;
    public bool antiGrenade = false;
    private bool specialShot = false;
    private bool specialUsed = false;
    IDamage player;

    public GameObject hitEffect;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject weaponHolder;

    private float nextShot = 0f;


    public Animator animator;
    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    void Update()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Shoot") && Time.time >= nextShot)
        {
            nextShot = Time.time + 1f / firerate;
            Shoot();
        }

        if (Input.GetButton("Special") && Time.time >= nextShot)
        {
            nextShot = Time.time + 1f / specialRate;
            Special();
        }
    }
    public void Shoot()
    {

        if (!gameManager.instance.isPaused)
        {
            muzzleFlash.Play();
            audioSource.PlayOneShot(shotSound);
            currentAmmo--;
            antiGrenade = true;
            RaycastHit hit;
            if (Physics.Raycast(shootCam.transform.position, shootCam.transform.forward, out hit, range))
            {
                Debug.Log(hit.transform.name);

                IDamage damageable = hit.collider.GetComponent<IDamage>();

                if (damageable != null && hit.transform != transform && damageable != player)
                {
                    damageable.hurtBaddies(damage);
                }

            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * force);
            }

            GameObject hitEffectGO = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(hitEffectGO, 1f);
        }
    }

    public void Special()
    {
        muzzleFlash.Play();
        audioSource.PlayOneShot(shotSound);
        currentAmmo--;
        antiGrenade = true;

        RaycastHit hit;
        if (Physics.Raycast(shootCam.transform.position, shootCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            IDamage damageable = hit.collider.GetComponent<IDamage>();

            if (damageable != null && hit.transform != transform && damageable != player)
            {
                damageable.hurtBaddies(damage);
            }

        }

        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForce(-hit.normal * force);
        }

        GameObject hitEffectGO = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(hitEffectGO, 1f);
        specialUsed = true;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("Reloading", true);
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

        isReloading = false;
        specialUsed = false;
    }


}
