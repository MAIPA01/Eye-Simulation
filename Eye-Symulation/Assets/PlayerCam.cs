using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public Vector2 sensitivity = new(.5f, .5f);

    public Vector2 constrains = new(-90f, 90f);

    public Transform orientation;
    
    Vector2 turn;

    private void OnValidate()
    {
        if (constrains.x > constrains.y)
        {
            float t = constrains.x;
            constrains.x = constrains.y;
            constrains.y = t;
        }
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            turn.x += Input.GetAxis("Mouse X") * sensitivity.x;
            turn.y += Input.GetAxis("Mouse Y") * sensitivity.y;

            turn.y = Mathf.Clamp(turn.y, constrains.x, constrains.y);

            // rotate cam and orientation
            transform.localRotation = Quaternion.Euler(-turn.y, 0, 0);
            orientation.rotation = Quaternion.Euler(0, turn.x, 0);
        }
    }
}
