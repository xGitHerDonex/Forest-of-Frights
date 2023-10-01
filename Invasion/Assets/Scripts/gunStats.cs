using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

//GunStats Object Script, define the stats for each gun here

public class gunStats : ScriptableObject
{
    [Header("----- Gun Stats -----")]
    public GunType gunType; // Add a field to specify the gun type.
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

    [SerializeField] public GameObject projectile;

    // Add an enumeration to specify the gun type.
    public enum GunType
    {
        Pistol,
        Railgun, // Add Railgun as a gun type.
        // Add more gun types as needed.
    }
}
