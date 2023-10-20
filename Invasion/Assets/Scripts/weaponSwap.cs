using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class weaponSwap : MonoBehaviour
{
    public int selectedWeapon = 0;
    [SerializeField] private Text ammoCurText;

    void Start()
    {
        //selectedWeapon = PlayerPrefs.GetInt("SelectedWeapon", 0);
        SelectWeapon();
        
    }

    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (!gameManager.instance.isPaused)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (selectedWeapon >= transform.childCount - 1)
                    selectedWeapon = 0;
                else
                    selectedWeapon++;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (selectedWeapon <= 0)
                    selectedWeapon = transform.childCount - 1;
                else
                    selectedWeapon--;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    selectedWeapon = i;
                    break;
                }
            }
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }
    //if (Input.GetKeyDown(KeyCode.Alpha1))
    //{
    //    selectedWeapon = 0;
    //}
    //if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
    //{
    //    selectedWeapon = 1;
    //}
    //if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
    //{
    //    selectedWeapon = 2;
    //}
    //if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
    //{
    //    selectedWeapon = 3;
    //}
    //    if (previousSelectedWeapon != selectedWeapon)
    //    {
    //     SelectWeapon();
    //    }
    //}

    void SelectWeapon()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform weapon = transform.GetChild(i);
            weapon.gameObject.SetActive(i == selectedWeapon);

            if (i == selectedWeapon)
            {
                firearm currentWeapon = weapon.GetComponent<firearm>();
                if (currentWeapon != null)
                {
                    if (currentWeapon.currentAmmo > 0)
                    {
                        if (currentWeapon.ammoCurText != null)
                        {
                            currentWeapon.ammoCurText.text = currentWeapon.currentAmmo.ToString();
                            currentWeapon.ammoMaxText.text = currentWeapon.maxAmmo.ToString();
                        }
                    }
                    else
                    {
                        if (currentWeapon.ammoCurText != null)
                        {
                            currentWeapon.ammoCurText.text = "0";
                            currentWeapon.ammoMaxText.text = "0";
                        }
                    }
                }
            }
        }
    }
}
