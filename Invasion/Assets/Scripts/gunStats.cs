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
    public ParticleSystem shootEffect;
    public AudioClip shotSound;
    public Transform gunMuzzle;
 
    [SerializeField] public bool isRailgun;
    [SerializeField] public GameObject projectile;
    [SerializeField] public float projectileSpeed;
    

}
