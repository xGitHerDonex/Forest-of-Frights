using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObjects : MonoBehaviour
{
    //Toggle switches in the inspector to set if the target is...
    //...floating or spinning, or both!
    public bool floatEnabled = false;
    public bool horizontalFloatEnabled = false;
    public bool spinEnabled = false;
    public float floatSpeed = 1.0f;
    public float floatDistance = 0.5f;
    public float spinSpeed = 30.0f;

    private Vector3 initialPosition;
 
    void Start()
    {
        //Sets the start position for the object to be manipulated
        initialPosition = transform.position;
    }

    
    void Update()
    {
        if (floatEnabled)
        {
            //If you turn on float, the object will float up and down
            float newY = initialPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatDistance;
        
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        }

        if (horizontalFloatEnabled)
        {
            //If you turn on float, the object will float up and down
            float newX = initialPosition.x + Mathf.Sin(Time.time * floatSpeed) * floatDistance;

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        }

        if (spinEnabled)
        {
            //if you turn on spin, the object will spin in around in a 360
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        }
    }
}
