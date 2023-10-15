using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class equipmentPickup : MonoBehaviour
{
    public enum EquipmentItem
    {
        None,
        EnergeticRing,
        CrimsonStone,
        TemporalRelic,
        Metabolizer,
        FlightX,
        Enhancer,
        NanoInfusor,
        PhaseShifter,
        ReflexGauntlet,
        AntiGravStone,
        Synthesizer,
        Vitalizer,
        ChronoGreaves,
        PowerInfusor,
        FarSight
    }

    [SerializeField] private EquipmentItem equipmentType;
    [SerializeField] private GameObject crimsonStoneUI;
    [SerializeField] private GameObject chronoGreavesUI;
    [SerializeField] private GameObject reflexGauntletUI;
    //[SerializeField] private playerController player;
    //[SerializeField] private GameObject energeticRingUI;

    //[SerializeField] private GameObject temportalRelicUI;
    //[SerializeField] private GameObject metabolizerUI;
    //[SerializeField] private GameObject flightxUI;
    //[SerializeField] private GameObject enhancerUI;
    //[SerializeField] private GameObject nanoInfusorUI;
    //[SerializeField] private GameObject phaseShifterUI;

    //[SerializeField] private GameObject antiGravStoneUI;
    //[SerializeField] private GameObject synthesizerUI;
    //[SerializeField] private GameObject vitalizerUI;

    //[SerializeField] private GameObject powerInfusorUI;
    //[SerializeField] private GameObject farSightUI;

    private playerController player;
    private void Awake()
    {
        
    }
    private void Start()
    {
        player = gameManager.instance.player.GetComponent<playerController>();
    }
    private void OnTriggerEnter( Collider other )
    {
        if (other.CompareTag("Player"))
        {
            pickupEquipment();
            Destroy(gameObject);
        }
    }

    public void pickupEquipment() 
    {
        switch (equipmentType)
        {


            case EquipmentItem.CrimsonStone: 

                {
                    player.ApplyPermanentHPBoost(50);
                    crimsonStoneUI.SetActive(true);
                    break;
                }


            case EquipmentItem.ChronoGreaves:
                {
                    chronoGreavesUI.SetActive(true);

                    if (chronoGreavesUI.activeSelf == true)
                    {
                        player.IncreasePlayerSpeed();
                    }
                    break;
                }
            case EquipmentItem.ReflexGauntlet:

                {
                    reflexGauntletUI.SetActive(true);
                    break;
                }
                //case EquipmentItem.EnergeticRing:

                //    {
                //        energeticRingUI.SetActive(true);
                //        gameManager.instance.playerScript.hasEnergeticRing = true;
                //        break;

                //    }
                //case EquipmentItem.TemporalRelic:

                //    {
                //        temportalRelicUI.SetActive(true);
                //        break;
                //    }


                //case EquipmentItem.Metabolizer:

                //    {
                //        metabolizerUI.SetActive(true);
                //        break;
                //    }


                //case EquipmentItem.FlightX:

                //    {
                //        flightxUI.SetActive(true);
                //        break;
                //    }


                //case EquipmentItem.Enhancer:

                //    {
                //        enhancerUI.SetActive(true);
                //        break;
                //    }


                //case EquipmentItem.NanoInfusor:

                //    {
                //        nanoInfusorUI.SetActive(true);
                //        break;
                //    }


                //case EquipmentItem.PhaseShifter:

                //    {
                //        phaseShifterUI.SetActive(true);
                //        break;
                //    }





                //case EquipmentItem.AntiGravStone:

                //    {
                //        antiGravStoneUI.SetActive(true);


                //        break;

                //    }


                //case EquipmentItem.Synthesizer:
                //    {
                //        synthesizerUI.SetActive(true);
                //        break;
                //    }


                //case EquipmentItem.Vitalizer:
                //    {
                //        vitalizerUI.SetActive(true);
                //        break;
                //    }





                //case EquipmentItem.PowerInfusor:
                //    {
                //        powerInfusorUI.SetActive(true);
                //        break;
                //    }


                //case EquipmentItem.FarSight:
                //    {
                //        farSightUI.SetActive(true);
                //        break;
                //    }




        }
    }

   
}
