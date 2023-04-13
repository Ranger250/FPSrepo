using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Look Sensitivity")]
    public float sensX;
    public float sensY;

    [Header("Clamping")]
    public float minY;
    public float maxY;

    [Header("Spectator")]
    public float spectatorMoveSpeed;

    [Header("Current")]
    public float rotX;
    public float rotY;
    public bool isSpectator;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        // get the mouse inputs
        rotX += Input.GetAxis("Mouse X") * sensX;
        rotY += Input.GetAxis("Mouse X") * sensY;

        rotY = Mathf.Clamp(rotY, minY, maxY);

        // if we are dead
        if (isSpectator)
        {
            // rotate the camera vertically
            transform.rotation = Quaternion.Euler(-rotY, rotX, 0);
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            float y = 0;
        }
        // if we are good
        else
        {

        }
    }

}
