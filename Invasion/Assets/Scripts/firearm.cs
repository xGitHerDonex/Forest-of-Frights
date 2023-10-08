using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class firearm : MonoBehaviour
{
    public int damage = 1;
    public float range = 10f;
    public float force = 30f;
    public float firerate = 15f;
    public float specialRate = 1f;

    public int maxAmmo = 10;
    public int currentAmmo;
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
    [SerializeField] public TextMeshProUGUI ammoCurText;
    [SerializeField] public TextMeshProUGUI ammoMaxText;

    private float nextShot = 0f;


    public Animator animator;
    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        //ammoCurText = gameManager.instance.getAmmoCurText();
        //ammoMaxText = gameManager.instance.getAmmoMaxText();    
    }

    private void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    private void Awake()
    {

        //ammoCurText = gameManager.instance.getAmmoCurText();
        //ammoMaxText = gameManager.instance.getAmmoMaxText();
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

            if (ammoCurText != null)
            {
                ammoCurText.text = currentAmmo.ToString();
            }
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

        if (ammoCurText != null)
        {
            ammoCurText.text = currentAmmo.ToString();
        }
    }

    IEnumerator Reload()
    {
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
            ammoCurText.text = currentAmmo.ToString();
            ammoMaxText.text = maxAmmo.ToString();
        }
    }

}
