using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject View; //irrelevant but i dont want to remove it cuz it might mess ssomething up
    public float Sensitivity;  //multiply your mouse inputs by that amount, x100
    float lookX, lookY;
    public Transform playerBody;
    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the input from the mouse and translate that into camera movement.
        lookX = Input.GetAxis("Mouse X") * (Sensitivity * 10) * Time.deltaTime;
        lookY = Input.GetAxis("Mouse Y") * (Sensitivity * 10) * Time.deltaTime;
        
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * lookX);

        //transform.rotation = Quaternion.Euler(lookX * Sensitivity,lookY * Sensitivity,0);
    }   
}
