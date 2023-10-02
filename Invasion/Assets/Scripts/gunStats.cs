using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

//GunStats Object Script, define the stats for each gun here

public class gunStats : ScriptableObject
{
    [Header("----- Gun Stats -----")]
    public float shootRate;
    public int shootDamage;
    public float recoilAmount;
    public int shootDist;
    public int ammoCur;
    public int ammoMax;

    public GameObject model;
    public ParticleSystem hitEffect;
    [SerializeField] public GameObject projectile;
    public ParticleSystem shootEffect;
    public AudioClip shotSound;
    public Transform gunMuzzle;
    public string gunName;

    

}
