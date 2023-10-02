using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static playerController;

public class ammoPickup : MonoBehaviour
{
    public playerController.AmmoType ammoType;  // Enum type

    // Available Ammo types
    public Dictionary<AmmoType, int> ammoAmounts = new Dictionary<AmmoType, int>()
{
    { AmmoType.PistolAmmo, 15 },
    { AmmoType.PulsarAmmo, 30 },
    { AmmoType.ShotgunAmmo, 5 },
    { AmmoType.RailgunAmmo, 3 },
    { AmmoType.RBFGAmmo, 1 }
};

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();

            if (player != null)
            {
                // Check if the ammo type is in the dictionary
                if (ammoAmounts.ContainsKey(player.ammoType))
                {
                    int amount = ammoAmounts[player.ammoType];

                    // Add ammo to the player's inventory
                    player.AddAmmo(player.ammoType, amount);
                    player.ammoUpdate();
                }

                Destroy(gameObject);
            }
        }
    }
}