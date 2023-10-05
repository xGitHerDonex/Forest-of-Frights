using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static playerController;

public class ammoPickup : MonoBehaviour
{

    public enum AmmoItem
    {
        None,
        PistolAmmo,
        ShotgunAmmo,
        RailgunAmmo,
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
    [SerializeField] private AmmoItem ammoType;
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

    public class AmmoItemData
    {
        public int pickupValue;

        public AmmoItemData(int pickupValue)
        {
            this.pickupValue = pickupValue;
        }
    }


}