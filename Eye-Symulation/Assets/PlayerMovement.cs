using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1.0f;

    public Transform orientation;

    float horizontal;
    float vertical;

    Vector3 moveDirection;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        moveDirection = orientation.forward * vertical + orientation.right * horizontal;

        if (moveDirection.sqrMagnitude != 0)
        {
            moveDirection.Normalize();
        }

        transform.position += moveDirection * speed * Time.deltaTime;
    }
}
