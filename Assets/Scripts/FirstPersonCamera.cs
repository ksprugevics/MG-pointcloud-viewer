using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private Transform camera_transform;
    private Vector3 position;
    private CharacterController controller;

    public float movement_speed;


    void Move()
    {
        Vector3 movement;
     
        // Move camera forwards, backwards, sideways
        movement = (camera_transform.forward * Input.GetAxis("Vertical") * movement_speed) +
        (transform.right * Input.GetAxis("Horizontal") * movement_speed);
    

        // Move camera up
        if(Input.GetKey(KeyCode.Space))
        {
            movement = (camera_transform.up * movement_speed);
        }

        // Move camera down
        if(Input.GetKey(KeyCode.LeftControl))
        {
            movement = (-camera_transform.up * movement_speed);
        }

        // "Sprint"
        if(Input.GetKey(KeyCode.LeftShift))
        {
            controller.Move(movement * 5 * Time.deltaTime);
        }
        else
        {
            controller.Move(movement * Time.deltaTime);
        }
    }


    void Rotate()
    {
        float yaw = camera_transform.eulerAngles.y;
        float pitch = camera_transform.eulerAngles.x;
        yaw += Input.GetAxis("Mouse X");
        pitch -= Input.GetAxis("Mouse Y");
        camera_transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }


    void CursorLock()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            switch(Cursor.lockState)
            {
                case CursorLockMode.None:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
                case CursorLockMode.Locked:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
            }
        }
    }

    void Start()
    {
        // Hides cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        camera_transform = GetComponent<Transform>();
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        CursorLock();
        Rotate();
        Move();
    }
}
