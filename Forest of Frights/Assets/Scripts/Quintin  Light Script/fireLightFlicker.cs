using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class fireLightFlicker : MonoBehaviour
{
    //stores the attached light. No serialized field for object as this script is only used on this light only
    Light firelight;

    //min and max values are time in seconds for a qquick flicker low values and for slow flicker higher values
    [Header("------Light Flicker Settings------")]
    [Range(0.01f, 1)][SerializeField] float minWait;
    [Range(0.01f, 1)][SerializeField] float maxWait;

    // Start is called before the first frame update
    void Start()
    {
        //gets the attached light
        firelight = GetComponent<Light>();
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
        yield return new WaitForSeconds(Random.Range(minWait,maxWait));
            firelight.enabled = !firelight.enabled;
        }
    }
}
