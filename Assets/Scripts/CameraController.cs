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
        rotY += Input.GetAxis("Mouse Y") * sensY;

        rotY = Mathf.Clamp(rotY, minY, maxY);

        // if we are dead
        if (isSpectator)
        {
            // rotate the camera vertically
            transform.rotation = Quaternion.Euler(-rotY, rotX, 0);


            float x = Input.GetAxis("Horizontal");
            float y = 0;
            float z = Input.GetAxis("Vertical");
            

            if (Input.GetKey(KeyCode.E))
            {
                y = 1;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                y = -1;
            }

            Vector3 dir = (transform.right * x) + (transform.up * y) + (transform.forward * z);

            transform.position += dir * spectatorMoveSpeed * Time.deltaTime;

        }
        // if we are good
        else
        {
            // rotate the camera vertically
            transform.localRotation = Quaternion.Euler(-rotY, 0, 0);
            transform.parent.rotation = Quaternion.Euler(transform.rotation.x, rotX, 0);
        }
    }

}
