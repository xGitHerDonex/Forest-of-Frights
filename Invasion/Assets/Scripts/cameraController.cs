using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    //Look Ability:  Limits up and down look range and adjusts sensitivity
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;

    //Float used for camera manipulation
    float xRotation;


    void Start()
    {
        //Hides the mouse cursor when starting the game
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        //Gets movement inputs
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        //Enables look up and down
        if (invertY)
        {
            xRotation += mouseY;
        } else
        {
            xRotation -= mouseY;
        }

        //Clamp camera rotation on the X-Axis
        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);

        //Rotates the camera on the X-Axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //Rotates the player on the Y-Axis
        transform.parent.Rotate(Vector3.up * mouseX);



    }
}
