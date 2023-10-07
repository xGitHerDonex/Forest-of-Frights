using System.Collections;
using UnityEngine;

public class firearm : MonoBehaviour
{
    public float damage = 1f;
    public float range = 10f;
    public float force = 30f;
    public float firerate = 15f;

    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;

    public Camera shootCam;
    public ParticleSystem muzzleFlash;
    public AudioClip shotSound;
    

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

        if (currentAmmo <=0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Shoot") && Time.time >= nextShot)
        {
            nextShot = Time.time + 1f / firerate;
            Shoot();
        }
    }
    public void Shoot()
    {
        muzzleFlash.Play();
        audioSource.PlayOneShot(shotSound);
        currentAmmo--;

        RaycastHit hit;
        if( Physics.Raycast(shootCam.transform.position, shootCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            targetDamage target = hit.transform.GetComponent<targetDamage>();
        if (target != null)
            {
                target.hurtBaddies(damage);
            }
        }

        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForce(-hit.normal * force);
        }

        GameObject hitEffectGO = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(hitEffectGO, 1f);
    }

    IEnumerator Reload()
    {
        isReloading = true;

        Debug.Log("Reloading");
        
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadTime -.25f);
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);

        currentAmmo = maxAmmo;
        
        isReloading = false;
    }

}
