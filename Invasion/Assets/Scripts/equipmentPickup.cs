using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private playerController player;
    [SerializeField] private GameObject energeticRingUI;
    [SerializeField] private GameObject crimsonStoneUI;
    [SerializeField] private GameObject temportalRelicUI;
    [SerializeField] private GameObject metabolizerUI;
    [SerializeField] private GameObject flightxUI;
    [SerializeField] private GameObject enhancerUI;
    [SerializeField] private GameObject nanoInfusorUI;
    [SerializeField] private GameObject phaseShifterUI;
    [SerializeField] private GameObject reflexGauntletUI;
    [SerializeField] private GameObject antiGravStoneUI;
    [SerializeField] private GameObject synthesizerUI;
    [SerializeField] private GameObject vitalizerUI;
    [SerializeField] private GameObject chronoGreavesUI;
    [SerializeField] private GameObject powerInfusorUI;
    [SerializeField] private GameObject farSightUI;







    private void OnTriggerEnter(Collider other)
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
            case EquipmentItem.EnergeticRing:
                if (player != null)
                {
                    energeticRingUI.SetActive(true);
                    player.hasEnergeticRing = true;
                }
                break;
            
            case EquipmentItem.CrimsonStone:

                if (player != null)
                {
                    player.ApplyPermanentStatBoost(10);
                    crimsonStoneUI.SetActive(true);
                }
                break;

            case EquipmentItem.TemporalRelic:
                if (temportalRelicUI != null)
                {
                    temportalRelicUI.SetActive(true);
                }
                break;

            case EquipmentItem.Metabolizer:
                if (metabolizerUI != null)
                {
                    metabolizerUI.SetActive(true);
                }
                break;

            case EquipmentItem.FlightX:
                if (flightxUI != null)
                {
                    flightxUI.SetActive(true);
                }
                break;

            case EquipmentItem.Enhancer:
                if (enhancerUI != null)
                {
                    enhancerUI.SetActive(true);
                }
                break;

            case EquipmentItem.NanoInfusor:
                if (nanoInfusorUI != null)
                {
                    nanoInfusorUI.SetActive(true);
                }
                break;

            case EquipmentItem.PhaseShifter:
                if (phaseShifterUI != null)
                {
                    phaseShifterUI.SetActive(true);
                }
                break;

            case EquipmentItem.ReflexGauntlet:
                if (reflexGauntletUI != null)
                {
                    reflexGauntletUI.SetActive(true);
                }
                break;

            case EquipmentItem.AntiGravStone:
                if (antiGravStoneUI != null)
                {
                    antiGravStoneUI.SetActive(true);
                }
                break;

            case EquipmentItem.Synthesizer:
                {
                    synthesizerUI.SetActive(true);
                }
                break;

            case EquipmentItem.Vitalizer:
                {
                    vitalizerUI.SetActive(true);
                }
                break;

            case EquipmentItem.ChronoGreaves:
                {
                    chronoGreavesUI.SetActive(true);
                }
                break;

            case EquipmentItem.PowerInfusor:
                {
                    powerInfusorUI.SetActive(true);
                }
                break;

            case EquipmentItem.FarSight:
                {
                    farSightUI.SetActive(true);
                }
                break;



        }
    }
}
