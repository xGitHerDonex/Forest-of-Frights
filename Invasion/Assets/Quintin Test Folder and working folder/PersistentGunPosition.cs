using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PersistentGunPosition : MonoBehaviour
{
    private int gunSelection;
    [SerializeField] private weaponSwap weaponholder;

    // Start is called before the first frame update
    void Awake()
    {
        LoadTheGunSlot();
        
    }

    // Update is called once per frame
    void Update()
    {
        //constantly keeps the gun postition updated
        GetGunPosition();
    }

    //GetSavedVariable the gun position from the weaponholder andd save it to a file
    private void GetGunPosition()
    {
        gunSelection = weaponholder.GiveMeTheWeaponYouAreUsing();
        SaveGunPosition();
    }

    private void SaveGunPosition()
    {
        int gp = gunSelection;
        PlayerPrefs.SetInt("Gun Position", gp);
        

    }

    //restores gun chose by the player through out the level
    private void LoadTheGunSlot()
    {
        int getGunSlot = PlayerPrefs.GetInt("Gun Position");
        weaponholder.SetMyWeapon(getGunSlot);
        
    }
}
