using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class fireLightFlicker : MonoBehaviour
{
    //stores the attached light. No serialized field for object as this script is only used on this light only
    Light flickeringlight;

    //min and max values are time in seconds for a qquick flicker low values and for slow flicker higher values
    [Header("------Light Flicker Settings------")]

    [Tooltip("Sets the minimum number of second that can be chosen by random. Smaller is a quicker light 0-10" )]
    [Range(0.01f, 10)][SerializeField] float minWait;

    [Tooltip("Sets the Maximum number of seconds that can be chosen by random. Smaller is a quicker light 0-10")]
    [Range(0.01f, 10)][SerializeField] float maxWait;

    // Start is called before the first frame update
    void Start()
    {
        //gets the attached light
        flickeringlight = GetComponent<Light>();
        StartCoroutine(Flicker());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Flicker()
    {

        while (true)
        {
        //wait for this amount of seconds before toggling the enabled status hidden or displayed
        yield return new WaitForSeconds(Random.Range(minWait ,maxWait));
            flickeringlight.enabled = !flickeringlight.enabled;
        }
    }
}
